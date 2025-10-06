using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

// ==== Explorer-like comparer (Windows) + fallback (macOS/Linux) ====
public static class FileSortUtil
{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    static extern int StrCmpLogicalW(string x, string y);

    public static readonly IComparer<string> ExplorerFileNameComparer =
        Comparer<string>.Create((a, b) =>
            StrCmpLogicalW(Path.GetFileName(a ?? string.Empty),
                           Path.GetFileName(b ?? string.Empty)));
#else
  public static readonly IComparer<string> ExplorerFileNameComparer =
      Comparer<string>.Create((a, b) => NaturalCompare(
          Path.GetFileName(a ?? string.Empty),
          Path.GetFileName(b ?? string.Empty)));

  static int NaturalCompare(string a, string b)
  {
    if (ReferenceEquals(a, b)) return 0;
    if (a is null) return -1; if (b is null) return 1;
    int ia = 0, ib = 0;
    while (ia < a.Length && ib < b.Length)
    {
      char ca = a[ia], cb = b[ib];
      bool da = char.IsDigit(ca), db = char.IsDigit(cb);
      if (da && db)
      {
        long va = 0, vb = 0; int sa = ia, sb = ib;
        while (ia < a.Length && char.IsDigit(a[ia])) { va = va * 10 + (a[ia] - '0'); ia++; }
        while (ib < b.Length && char.IsDigit(b[ib])) { vb = vb * 10 + (b[ib] - '0'); ib++; }
        if (va != vb) return va < vb ? -1 : 1;
        int lenA = ia - sa, lenB = ib - sb;
        if (lenA != lenB) return lenA < lenB ? -1 : 1; // "2" < "02"
      }
      else
      {
        int c = char.ToUpperInvariant(ca).CompareTo(char.ToUpperInvariant(cb));
        if (c != 0) return c;
        ia++; ib++;
      }
    }
    return (a.Length - ia).CompareTo(b.Length - ib);
  }
#endif
}
