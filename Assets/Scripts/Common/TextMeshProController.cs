using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProController : MonoBehaviour
{
    [Header("Curve Settings")]
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private bool useCurve = false;
    [SerializeField] private float radius = 50f;
    [SerializeField] private float arcAngle = 180f;

    [Header("Animation Settings")]
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float delayBetweenChars = 0.1f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Coroutine currentAnimCoroutine;
    private Vector3[] originalVertices;

    private void Awake()
    {
        if (!textMeshPro) textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        // Chuẩn bị mesh/curve trước khi animate
        if (useCurve) ApplyCurve();
        else textMeshPro.ForceMeshUpdate();

        if (playOnEnable)
            PlayScaleAnimation();
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    /// <summary>
    /// Bẻ cong chữ theo cung tròn (áp vào mesh hiện tại).
    /// </summary>
    private void ApplyCurve()
    {
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0 || textInfo.meshInfo.Length == 0) return;

        var meshInfo = textInfo.meshInfo[0];

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

            int vertexIndex = charInfo.vertexIndex;

            float t = (textInfo.characterCount == 1) ? 0.5f : (float)i / (textInfo.characterCount - 1);
            float angle = Mathf.Lerp(-arcAngle * 0.5f, arcAngle * 0.5f, t) * Mathf.Deg2Rad;

            // Tính tâm 4 đỉnh của ký tự
            Vector3 center = Vector3.zero;
            for (int j = 0; j < 4; j++) center += meshInfo.vertices[vertexIndex + j];
            center *= 0.25f;

            // Xoay ký tự quanh tâm và “ép” nó lên cung tròn
            float cosA = Mathf.Cos(angle);
            float sinA = Mathf.Sin(angle);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = meshInfo.vertices[vertexIndex + j];

                // Đưa về tâm
                v -= center;

                // Xoay quanh tâm theo góc
                float rx = v.x * cosA - v.y * sinA;
                float ry = v.x * sinA + v.y * cosA;
                v = new Vector3(rx, ry, v.z);

                // Đưa lên cung tròn có bán kính radius
                float x = v.x;
                float y = v.y + radius;

                float nx = x * cosA - y * sinA;
                float ny = x * sinA + y * cosA;

                meshInfo.vertices[vertexIndex + j] = new Vector3(nx, ny - radius, v.z) + center;
            }
        }

        textMeshPro.UpdateVertexData();
    }

    /// <summary>
    /// Public API để chạy hiệu ứng scale từng ký tự.
    /// </summary>
    public void PlayScaleAnimation(float customDuration = -1f, float customDelay = -1f)
    {
        float duration = customDuration > 0 ? customDuration : animDuration;
        float delay = customDelay > 0 ? customDelay : delayBetweenChars;

        // Hủy cái cũ (nếu đang chạy)
        StopAnimation();

        // Khởi tạo lại dữ liệu gốc trước khi animate
        PrepareOriginalVertices();

        // Chạy coroutine
        currentAnimCoroutine = StartCoroutine(AnimateScaleSequence(duration, delay));
    }

    /// <summary>
    /// Dừng hiệu ứng và khôi phục mesh.
    /// </summary>
    public void StopAnimation()
    {
        if (currentAnimCoroutine != null)
        {
            StopCoroutine(currentAnimCoroutine);
            currentAnimCoroutine = null;
        }

        // Kill toàn bộ tween gắn với object này
        DOTween.Kill(this);

        // Khôi phục vertices nếu có
        if (originalVertices != null && textMeshPro && textMeshPro.textInfo.meshInfo.Length > 0)
        {
            var meshInfo = textMeshPro.textInfo.meshInfo[0];
            if (meshInfo.vertices != null && meshInfo.vertices.Length == originalVertices.Length)
            {
                System.Array.Copy(originalVertices, meshInfo.vertices, originalVertices.Length);
                // textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }
        }
    }

    private void OnDestroy()
    {
        StopAnimation();
    }

    // =========================
    //      Core Animation
    // =========================

    private void PrepareOriginalVertices()
    {
        // Nếu có curve, giữ mesh đã bẻ cong làm "gốc" để scale từ đó
        if (useCurve) ApplyCurve();
        else textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.meshInfo.Length == 0) return;

        var meshInfo = textInfo.meshInfo[0];
        if (meshInfo.vertices == null) return;

        if (originalVertices == null || originalVertices.Length != meshInfo.vertices.Length)
            originalVertices = new Vector3[meshInfo.vertices.Length];

        System.Array.Copy(meshInfo.vertices, originalVertices, meshInfo.vertices.Length);
    }

    private IEnumerator AnimateScaleSequence(float duration, float delay)
    {
        if (!textMeshPro) yield break;
        textMeshPro.ForceMeshUpdate();

        var textInfo = textMeshPro.textInfo;
        if (textInfo.characterCount == 0 || textInfo.meshInfo.Length == 0) yield break;

        var meshInfo = textInfo.meshInfo[0];
        if (meshInfo.vertices == null) yield break;

        // Đưa toàn bộ ký tự về scale 0 trước
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var ch = textInfo.characterInfo[i];
            if (!ch.isVisible) continue;

            ApplyScaleToCharacter(ch.vertexIndex, GetCharacterCenter(ch.vertexIndex), 0f);
        }
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        // Lần lượt scale lên theo delay
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var ch = textInfo.characterInfo[i];
            if (!ch.isVisible) continue;

            AnimateCharacterScale(ch.vertexIndex, duration);

            if (delay > 0f)
                yield return new WaitForSeconds(delay);
            else
                yield return null; // 1 frame để mượt
        }

        currentAnimCoroutine = null;
    }

    private void AnimateCharacterScale(int vertexIndex, float duration)
    {
        Vector3 center = GetCharacterCenter(vertexIndex);

        // Dùng DOTween để nội suy "scale" từ 0 -> 1, set target = this để dễ Kill
        DOTween.To(() => 0f, s =>
            {
                ApplyScaleToCharacter(vertexIndex, center, s);
                textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            }, 1f, duration)
            .SetEase(scaleCurve)
            .SetTarget(this);
    }

    private Vector3 GetCharacterCenter(int vertexIndex)
    {
        Vector3 c = Vector3.zero;
        for (int j = 0; j < 4; j++)
            c += originalVertices[vertexIndex + j];
        return c * 0.25f;
    }

    private void ApplyScaleToCharacter(int vertexIndex, Vector3 center, float scale)
    {
        var meshInfo = textMeshPro.textInfo.meshInfo[0];
        for (int j = 0; j < 4; j++)
        {
            Vector3 v = originalVertices[vertexIndex + j];
            Vector3 dir = v - center;
            meshInfo.vertices[vertexIndex + j] = center + dir * scale;
        }
    }
}
