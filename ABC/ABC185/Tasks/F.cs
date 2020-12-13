using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Tasks
{
    public class F
    {
        public static void Main(string[] args)
        {
            var sw = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = false };
            Console.SetOut(sw);
            Solve();
            Console.Out.Flush();
        }

        public static void Solve()
        {
            var (N, Q) = Scanner.Scan<int, int>();
            var A = Scanner.ScanEnumerable<int>().ToArray();
            var lst = new LazySegmentTree<Monoid, long>(A.Select(x => new Monoid(x, 1)), new Oracle());
            while (Q-- > 0)
            {
                var (T, X, Y) = Scanner.Scan<int, int, int>();
                X--;
                if (T == 1)
                {
                    var before = lst.Get(X);
                    lst.Set(X, new Monoid(before.Value ^ Y, before.Size));
                }
                else
                {
                    Console.WriteLine(lst.Query(X, Y).Value);
                }
            }
        }

        public readonly struct Monoid
        {
            public readonly long Value;
            public readonly long Size;
            public Monoid(long value, long size) => (Value, Size) = (value, size);
        }

        public class Oracle : IOracle<Monoid, long>
        {
            public long MapIdentity => 0;

            public Monoid MonoidIdentity => new Monoid(0, 0);

            public long Compose(in long f, in long g)
            {
                return f == 0 ? g : f;
            }

            public Monoid Map(in long f, in Monoid x)
            {
                if (f != 0) return new Monoid((x.Size * f), x.Size);
                return x;
            }

            public Monoid Operate(in Monoid a, in Monoid b)
            {
                return new Monoid(a.Value ^ b.Value, a.Size + b.Size);
            }
        }

        public interface IOracle<TMonoid> where TMonoid : struct
        {
            TMonoid MonoidIdentity { get; }
            TMonoid Operate(in TMonoid a, in TMonoid b);
        }
        public interface IOracle<TMonoid, TMap> : IOracle<TMonoid> where TMap : struct where TMonoid : struct
        {
            TMap MapIdentity { get; }
            TMonoid Map(in TMap f, in TMonoid x);
            TMap Compose(in TMap f, in TMap g);
        }


        public class LazySegmentTree<TMonoid, TMap> where TMonoid : struct where TMap : struct
        {
            private readonly int _length;
            private readonly int _size;
            private readonly int _log;
            private readonly TMonoid[] _data;
            private readonly TMap[] _lazy;
            private readonly TMonoid _monoidId;
            private readonly TMap _mapId;
            private readonly IOracle<TMonoid, TMap> _oracle;
            public LazySegmentTree(int length, IOracle<TMonoid, TMap> oracle)
                : this(Enumerable.Repeat(oracle.MonoidIdentity, length), oracle)
            {
            }
            public LazySegmentTree(IEnumerable<TMonoid> data, IOracle<TMonoid, TMap> oracle)
            {
                var d = data.ToArray();
                _length = d.Length;
                _oracle = oracle;
                _monoidId = _oracle.MonoidIdentity;
                _mapId = _oracle.MapIdentity;
                while (1 << _log < _length) _log++;
                _size = 1 << _log;
                _data = new TMonoid[_size << 1];
                Array.Fill(_data, _monoidId);
                _lazy = new TMap[_size];
                Array.Fill(_lazy, _mapId);
                d.CopyTo(_data, _size);
                for (var i = _size - 1; i >= 1; i--) Update(i);
            }
            public void Set(int index, in TMonoid monoid)
            {
                if (index < 0 || _length <= index) throw new ArgumentOutOfRangeException(nameof(index));
                index += _size;
                for (var i = _log; i >= 1; i--) Push(index >> i);
                _data[index] = monoid;
                for (var i = 1; i <= _log; i++) Update(index >> i);
            }
            public TMonoid Get(int index)
            {
                if (index < 0 || _length <= index) throw new ArgumentOutOfRangeException(nameof(index));
                index += _size;
                for (var i = _log; i >= 1; i--) Push(index >> i);
                return _data[index];
            }
            public TMonoid Query(int left, int right)
            {
                if (left < 0 || right < left || _length < right) throw new ArgumentOutOfRangeException();
                if (left == right) return _monoidId;
                left += _size;
                right += _size;
                for (var i = _log; i >= 1; i--)
                {
                    if ((left >> i) << i != left) Push(left >> i);
                    if ((right >> i) << i != right) Push(right >> i);
                }
                var (sml, smr) = (_monoidId, _monoidId);
                while (left < right)
                {
                    if ((left & 1) == 1) sml = _oracle.Operate(sml, _data[left++]);
                    if ((right & 1) == 1) smr = _oracle.Operate(_data[--right], smr);
                    left >>= 1;
                    right >>= 1;
                }
                return _oracle.Operate(sml, smr);
            }
            public TMonoid QueryToAll() => _data[1];
            public void Apply(int index, TMap map)
            {
                if (index < 0 || _length <= index) throw new ArgumentOutOfRangeException(nameof(index));
                index += _size;
                for (var i = _log; i >= 1; i--) Push(index >> i);
                _data[index] = _oracle.Map(map, _data[index]);
                for (var i = 1; i <= _log; i++) Update(index >> i);
            }
            public void Apply(int left, int right, in TMap map)
            {
                if (left < 0 || right < left || _length < right) throw new ArgumentOutOfRangeException();
                if (left == right) return;
                left += _size;
                right += _size;
                for (var i = _log; i >= 1; i--)
                {
                    if ((left >> i) << i != left) Push(left >> i);
                    if ((right >> i) << i != right) Push((right - 1) >> i);
                }
                var (l, r) = (left, right);
                while (l < r)
                {
                    if ((l & 1) == 1) ApplyToAll(l++, map);
                    if ((r & 1) == 1) ApplyToAll(--r, map);
                    l >>= 1;
                    r >>= 1;
                }
                for (var i = 1; i <= _log; i++)
                {
                    if ((left >> i) << i != left) Update(left >> i);
                    if ((right >> i) << i != right) Update((right - 1) >> i);
                }
            }
            public int MaxRight(int left, Predicate<TMonoid> predicate)
            {
                if (left < 0 || _length < left) throw new ArgumentOutOfRangeException(nameof(left));
                if (predicate == null) throw new ArgumentNullException(nameof(predicate));
                if (!predicate(_monoidId)) throw new ArgumentException(nameof(predicate));
                if (left == _length) return _length;
                left += _size;
                for (var i = _log; i >= 1; i--) Push(left >> i);
                var sm = _monoidId;
                do
                {
                    while ((left & 1) == 0) left >>= 1;
                    if (!predicate(_oracle.Operate(sm, _data[left])))
                    {
                        while (left < _size)
                        {
                            Push(left);
                            left <<= 1;
                            var tmp = _oracle.Operate(sm, _data[left]);
                            if (!predicate(tmp)) continue;
                            sm = tmp;
                            left++;
                        }
                        return left - _size;
                    }
                    sm = _oracle.Operate(sm, _data[left]);
                    left++;
                } while ((left & -left) != left);
                return _length;
            }
            public int MinLeft(int right, Predicate<TMonoid> predicate)
            {
                if (right < 0 || _length < right) throw new ArgumentOutOfRangeException(nameof(right));
                if (predicate == null) throw new ArgumentNullException(nameof(predicate));
                if (!predicate(_monoidId)) throw new ArgumentException(nameof(predicate));
                if (right == 0) return 0;
                right += _size;
                for (var i = _log; i >= 1; i--) Push((right - 1) >> i);
                var sm = _monoidId;
                do
                {
                    right--;
                    while (right > 1 && (right & 1) == 1) right >>= 1;
                    if (!predicate(_oracle.Operate(_data[right], sm)))
                    {
                        while (right < _size)
                        {
                            Push(right);
                            right = (right << 1) + 1;
                            var tmp = _oracle.Operate(_data[right], sm);
                            if (!predicate(tmp)) continue;
                            sm = tmp;
                            right--;
                        }
                        return right + 1 - _size;
                    }
                    sm = _oracle.Operate(_data[right], sm);
                } while ((right & -right) != right);
                return 0;
            }
            private void Update(int k) => _data[k] = _oracle.Operate(_data[k << 1], _data[(k << 1) + 1]);
            private void ApplyToAll(int k, in TMap map)
            {
                _data[k] = _oracle.Map(map, _data[k]);
                if (k < _size) _lazy[k] = _oracle.Compose(map, _lazy[k]);
            }
            private void Push(int k)
            {
                ApplyToAll(k << 1, _lazy[k]);
                ApplyToAll((k << 1) + 1, _lazy[k]);
                _lazy[k] = _mapId;
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
            public static (T1, T2, T3, T4, T5, T6) Scan<T1, T2, T3, T4, T5, T6>() => (Next<T1>(), Next<T2>(), Next<T3>(), Next<T4>(), Next<T5>(), Next<T6>());
            public static IEnumerable<T> ScanEnumerable<T>() => Console.ReadLine().Trim().Split(" ").Select(x => (T)Convert.ChangeType(x, typeof(T)));
        }
    }
}
