using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tasks
{
    public class I
    {
        static void Main(string[] args)
        {
            var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
            Console.SetOut(sw);
            Solve();
            Console.Out.Flush();
        }

        public static void Solve()
        {
            var S = Scanner.Scan<string>();
            var sa = StringAlgorithm.SuffixArray(S);
            var answer = (long)S.Length * (S.Length + 1) / 2;
            foreach (var x in StringAlgorithm.LCPArray(S, sa))
            {
                answer -= x;
            }
            Console.WriteLine(answer);
        }

        public static class StringAlgorithm
        {
            public static IEnumerable<int> SuffixArray(string str)
            {
                return SuffixArrayInducedSorting(str.Select(x => x - 0), 255);
            }

            public static IEnumerable<int> SuffixArray<T>(IEnumerable<T> items)
            {
                var s = items.ToArray();
                var n = s.Length;
                var idx = Enumerable.Range(0, n).ToArray();
                Array.Sort(idx, (x, y) => Comparer<T>.Default.Compare(s[x], s[y]));
                var s2 = new int[n];
                var now = 0;
                for (var i = 0; i < n; i++)
                {
                    if (i > 0 && !Equals(s[idx[i - 1]], s[idx[i]])) now++;
                    s2[idx[i]] = now;
                }

                return SuffixArrayInducedSorting(s2, now);
            }

            public static IEnumerable<int> SuffixArray(IEnumerable<int> items, int upper)
            {
                if (items == null) throw new ArgumentException(nameof(items));
                if (upper < 0) throw new ArgumentException(nameof(upper));
                var s = items.ToArray();
                if (s.Any(x => x < 0 || upper <= x)) throw new ArgumentException(nameof(items));
                return SuffixArrayInducedSorting(s, upper);
            }

            public static IEnumerable<int> LCPArray(string str, IEnumerable<int> suffixArray)
            {
                return LCPArray(str.Select(x => x - 0), suffixArray);
            }

            public static IEnumerable<int> LCPArray<T>(IEnumerable<T> items, IEnumerable<int> suffixArray)
            {
                var s = items.ToArray();
                var sa = suffixArray.ToArray();
                var n = s.Length;
                if (n < 1) throw new ArgumentException(nameof(items));
                var rnk = new int[n];
                for (var i = 0; i < rnk.Length; i++) rnk[sa[i]] = i;
                var lcp = new int[n - 1];
                var h = 0;
                for (var i = 0; i < n; i++)
                {
                    if (h > 0) h--;
                    if (rnk[i] == 0) continue;
                    var j = sa[rnk[i] - 1];
                    while (j + h < n && i + h < n)
                    {
                        if (!Equals(s[j + h], s[i + h])) break;
                        h++;
                    }

                    lcp[rnk[i] - 1] = h;
                }

                return lcp;
            }

            public static IEnumerable<int> ZAlgorithm(string str)
            {
                return ZAlgorithm(str.Select(x => x - 0));
            }

            public static IEnumerable<int> ZAlgorithm<T>(IEnumerable<T> items)
            {
                var s = items.ToArray();
                var n = s.Length;
                if (n == 0) return new List<int>();
                var z = new[] { 0 };
                for (int i = 1, j = 0; i < n; i++)
                {
                    z[i] = (j + z[j] <= i) ? 0 : System.Math.Min(j + z[j] - i, z[i - j]);
                    while (i + z[i] < n && Equals(s[z[i]], s[i + z[i]])) z[i]++;
                    if (j + z[j] < i + z[i]) j = i;
                }

                z[0] = n;
                return z;
            }

            private static IEnumerable<int> SuffixArrayInducedSorting(IEnumerable<int> items, int upper,
                int naive = 10, int doubling = 40)
            {
                var s = items.ToArray();
                var n = s.Length;
                switch (n)
                {
                    case 0: return new int[0];
                    case 1: return new[] { 0 };
                    case 2: return s[0] < s[1] ? new[] { 0, 1 } : new[] { 1, 0 };
                }

                if (n < naive) return SuffixArrayNaive(s);
                if (n < doubling) return SuffixArrayDoubling(s);

                var sa = new int[n];
                var ls = new bool[n];
                for (var i = n - 2; i >= 0; i--)
                {
                    ls[i] = s[i] == s[i + 1] ? ls[i + 1] : s[i] < s[i + 1];
                }

                var sumL = new int[upper + 1];
                var sumS = new int[upper + 1];
                for (var i = 0; i < n; i++)
                {
                    if (!ls[i]) sumS[s[i]]++;
                    else sumL[s[i] + 1]++;
                }

                for (var i = 0; i <= upper; i++)
                {
                    sumS[i] += sumL[i];
                    if (i < upper) sumL[i + 1] += sumS[i];
                }

                void Induce(IEnumerable<int> ilms)
                {
                    sa = Enumerable.Repeat(-1, sa.Length).ToArray();
                    var buffer = new int[upper + 1];
                    sumS.CopyTo(buffer, 0);
                    foreach (var d in ilms)
                    {
                        if (d == n) continue;
                        sa[buffer[s[d]]++] = d;
                    }

                    sumL.CopyTo(buffer, 0);
                    sa[buffer[s[n - 1]]++] = n - 1;
                    for (var i = 0; i < n; i++)
                    {
                        var v = sa[i];
                        if (v >= 1 && !ls[v - 1]) sa[buffer[s[v - 1]]++] = v - 1;
                    }

                    sumL.CopyTo(buffer, 0);
                    for (var i = n - 1; i >= 0; i--)
                    {
                        var v = sa[i];
                        if (v >= 1 && ls[v - 1])
                        {
                            sa[--buffer[s[v - 1] + 1]] = v - 1;
                        }
                    }
                }

                var lmsMap = Enumerable.Repeat(-1, n + 1).ToArray();
                var m = 0;
                for (var i = 1; i < n; i++)
                {
                    if (!ls[i - 1] && ls[i]) lmsMap[i] = m++;
                }

                var lms = new List<int>();
                for (var i = 1; i < n; i++)
                {
                    if (!ls[i - 1] && ls[i]) lms.Add(i);
                }

                Induce(lms);

                if (m <= 0) return sa;

                var sortedLms = sa.Where(x => lmsMap[x] != -1).ToArray();
                var recS = new int[m];
                var recUpper = 0;
                recS[lmsMap[sortedLms[0]]] = 0;
                for (var i = 1; i < m; i++)
                {
                    var l = sortedLms[i - 1];
                    var r = sortedLms[i];
                    var el = lmsMap[l] + 1 < m ? lms[lmsMap[l] + 1] : n;
                    var er = lmsMap[r] + 1 < m ? lms[lmsMap[r] + 1] : n;
                    var isSame = true;
                    if (el - l != er - r) isSame = false;
                    else
                    {
                        while (l < el && s[l] == s[r])
                        {
                            l++;
                            r++;
                        }

                        if (l == n || s[l] != s[r]) isSame = false;
                    }

                    if (!isSame) recUpper++;
                    recS[lmsMap[sortedLms[i]]] = recUpper;
                }

                var recSa = SuffixArrayInducedSorting(recS, recUpper, naive, doubling).ToArray();
                for (var i = 0; i < m; i++)
                {
                    sortedLms[i] = lms[recSa[i]];
                }

                Induce(sortedLms);
                return sa;
            }

            private static IEnumerable<int> SuffixArrayNaive(IEnumerable<int> items)
            {
                var s = items.ToArray();
                var n = s.Length;
                var sa = Enumerable.Range(0, n).ToArray();

                int Compare(int x, int y)
                {
                    var comparer = Comparer<int>.Default;
                    if (x == y) return 0;
                    while (x < n && y < n)
                    {
                        if (s[x] != s[y]) return comparer.Compare(s[x], s[y]);
                        x++;
                        y++;
                    }

                    return comparer.Compare(x, n);
                }

                Array.Sort(sa, Compare);
                return sa;
            }

            private static IEnumerable<int> SuffixArrayDoubling(IEnumerable<int> items)
            {
                var rnk = items.ToArray();
                var n = rnk.Length;
                var sa = Enumerable.Range(0, n).ToArray();

                var tmp = new int[n];
                for (var k = 1; k < n; k *= 2)
                {
                    int Compare(int x, int y)
                    {
                        var comparer = Comparer<int>.Default;
                        if (rnk[x] != rnk[y]) return comparer.Compare(rnk[x], rnk[y]);
                        var rx = x + k < n ? rnk[x + k] : -1;
                        var ry = y + k < n ? rnk[y + k] : -1;
                        return comparer.Compare(rx, ry);
                    }

                    Array.Sort(sa, Compare);
                    tmp[sa[0]] = 0;
                    for (var i = 1; i < n; i++)
                    {
                        tmp[sa[i]] = tmp[sa[i - 1]] + (Compare(sa[i - 1], sa[i]) < 0 ? 1 : 0);
                    }

                    (tmp, rnk) = (rnk, tmp);
                }

                return sa;
            }
        }

        public static class Scanner
        {
            private static Queue<string> queue = new Queue<string>();
            public static T Next<T>()
            {
                if (!queue.Any()) foreach (var item in Console.ReadLine().Trim().Split(" ")) queue.Enqueue(item);
                return (T)Convert.ChangeType(queue.Dequeue(), typeof(T));
            }
            public static T Scan<T>() => Next<T>();
            public static (T1, T2) Scan<T1, T2>() => (Next<T1>(), Next<T2>());
            public static (T1, T2, T3) Scan<T1, T2, T3>() => (Next<T1>(), Next<T2>(), Next<T3>());
            public static (T1, T2, T3, T4) Scan<T1, T2, T3, T4>() => (Next<T1>(), Next<T2>(), Next<T3>(), Next<T4>());
            public static (T1, T2, T3, T4, T5) Scan<T1, T2, T3, T4, T5>() => (Next<T1>(), Next<T2>(), Next<T3>(), Next<T4>(), Next<T5>());
            public static IEnumerable<T> ScanEnumerable<T>() => Console.ReadLine().Trim().Split(" ").Select(x => (T)Convert.ChangeType(x, typeof(T)));
        }
    }
}
