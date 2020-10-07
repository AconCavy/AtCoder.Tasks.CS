using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tasks
{
    public class E
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
            var N = Scanner.Scan<int>();
            var P = Scanner.ScanEnumerable<long>().ToArray();
            var A = new (int i, long p)[N];
            for (var i = 0; i < N; i++) A[i] = (i + 1, P[i]);
            Array.Sort(A, (x, y) => y.p.CompareTo(x.p));
            var stMin = new SegmentTree<int>(N + 1, Math.Min, N + 1);
            var stMax = new SegmentTree<int>(N + 1, Math.Max, 0);
            var answer = 0L;
            for (var ph = 0; ph < N; ph++)
            {
                var (i, p) = A[ph];
                var x = stMax.Query(0, i);
                var w = stMax.Query(0, Math.Max(0, x));
                stMax.Set(i, i);

                var y = stMin.Query(i + 1, N + 1);
                var z = stMin.Query(Math.Min(y + 1, N + 1), N + 1);
                stMin.Set(i, i);

                answer += p * (x - w) * (y - i);
                answer += p * (i - x) * (z - y);
            }

            Console.WriteLine(answer);
        }

        public class SegmentTree<TMonoid>
        {
            private readonly int _n;
            private readonly int _size;
            private readonly int _log;
            private readonly TMonoid[] _data;
            private readonly Func<TMonoid, TMonoid, TMonoid> _operation;
            private readonly TMonoid _monoidId;
            public SegmentTree(int n, Func<TMonoid, TMonoid, TMonoid> operation, TMonoid monoidId)
                : this(Enumerable.Repeat(monoidId, n), operation, monoidId)
            {
            }
            public SegmentTree(IEnumerable<TMonoid> data, Func<TMonoid, TMonoid, TMonoid> operation, TMonoid monoidId)
            {
                var d = data.ToArray();
                _n = d.Length;
                _operation = operation;
                _monoidId = monoidId;
                while (1 << _log < _n) _log++;
                _size = 1 << _log;
                _data = Enumerable.Repeat(monoidId, _size << 1).ToArray();
                d.CopyTo(_data, _size);
                for (var i = _size - 1; i >= 1; i--) Update(i);
            }
            public void Set(int p, TMonoid x)
            {
                if (p < 0 || _n <= p) throw new IndexOutOfRangeException(nameof(p));
                p += _size;
                _data[p] = x;
                for (var i = 1; i <= _log; i++) Update(p >> i);
            }
            public TMonoid Get(int p)
            {
                if (p < 0 || _n <= p) throw new IndexOutOfRangeException(nameof(p));
                return _data[p + _size];
            }
            public TMonoid Query(int l, int r)
            {
                if (l < 0 || r < l || _n < r) throw new IndexOutOfRangeException();
                var (sml, smr) = (_monoidId, _monoidId);
                l += _size;
                r += _size;
                while (l < r)
                {
                    if ((l & 1) == 1) sml = _operation(sml, _data[l++]);
                    if ((r & 1) == 1) smr = _operation(_data[--r], smr);
                    l >>= 1;
                    r >>= 1;
                }
                return _operation(sml, smr);
            }
            public TMonoid QueryToAll() => _data[1];
            public int MaxRight(int l, Func<TMonoid, bool> func)
            {
                if (l < 0 || _n < l) throw new IndexOutOfRangeException(nameof(l));
                if (!func(_monoidId)) throw new ArgumentException(nameof(func));
                if (l == _n) return _n;
                l += _size;
                var sm = _monoidId;
                do
                {
                    while ((l & 1) == 0) l >>= 1;
                    if (!func(_operation(sm, _data[l])))
                    {
                        while (l < _size)
                        {
                            l <<= 1;
                            var tmp = _operation(sm, _data[l]);
                            if (!func(tmp)) continue;
                            sm = tmp;
                            l++;
                        }
                        return l - _size;
                    }
                    sm = _operation(sm, _data[l]);
                    l++;
                } while ((l & -l) != l);
                return _n;
            }
            public int MinLeft(int r, Func<TMonoid, bool> func)
            {
                if (r < 0 || _n < r) throw new IndexOutOfRangeException(nameof(r));
                if (!func(_monoidId)) throw new ArgumentException(nameof(func));
                if (r == 0) return 0;
                r += _size;
                var sm = _monoidId;
                do
                {
                    r--;
                    while (r > 1 && (r & 1) == 1) r >>= 1;
                    if (!func(_operation(_data[r], sm)))
                    {
                        while (r < _size)
                        {
                            r = (r << 1) + 1;
                            var tmp = _operation(_data[r], sm);
                            if (!func(tmp)) continue;
                            sm = tmp;
                            r--;
                        }
                        return r + 1 - _size;
                    }
                    sm = _operation(_data[r], sm);
                } while ((r & -r) != r);
                return 0;
            }
            private void Update(int k) => _data[k] = _operation(_data[k << 1], _data[(k << 1) + 1]);
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
