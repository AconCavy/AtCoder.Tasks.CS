using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.CompilerServices;


namespace Tasks
{
    public class F
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
            var (N, M) = Scanner.Scan<int, int>();
            var A = Scanner.ScanEnumerable<long>().Select(x => new MInt(x)).ToArray();
            var B = Scanner.ScanEnumerable<long>().Select(x => new MInt(x)).ToArray();
            var c = Convolution.Execute(A, B);
            Console.WriteLine(string.Join(" ", c));
        }

        public static class Convolution
        {
            private static MInt[] _sumE;
            private static MInt[] _sumIE;
            private static int _primitiveRoot;

            public static IEnumerable<MInt> Execute(IEnumerable<MInt> a, IEnumerable<MInt> b)
            {
                var (a1, b1) = (a.ToArray(), b.ToArray());
                var (n, m) = (a1.Length, b1.Length);
                var ret = new MInt[n + m - 1];
                if (System.Math.Min(n, m) <= 60)
                {
                    for (var i = 0; i < n; i++)
                    {
                        for (var j = 0; j < m; j++)
                        {
                            ret[i + j] += a1[i] * b1[j];
                        }
                    }

                    return ret;
                }

                var z = 1 << CeilPower2(n + m - 1);
                Array.Resize(ref a1, z);
                Array.Resize(ref b1, z);
                a1 = Butterfly(a1).ToArray();
                b1 = Butterfly(b1).ToArray();
                for (var i = 0; i < a1.Length; i++) a1[i] *= b1[i];
                ret = ButterflyInverse(a1).ToArray();
                Array.Resize(ref ret, n + m - 1);
                return ret.Select(x => x / z);
            }

            private static void Initialize()
            {
                if (_sumE != null && _sumIE != null) return;
                var m = MInt.Mod;
                _primitiveRoot = PrimitiveRoot(m);
                _sumE = new MInt[30];
                _sumIE = new MInt[30];
                var es = new MInt[30];
                var ies = new MInt[30];
                var count2 = BitScanForward(m - 1);
                var e = new MInt(_primitiveRoot).Power((m - 1) >> count2);
                var ie = e.Inverse();
                for (var i = count2; i >= 2; i--)
                {
                    es[i - 2] = e;
                    ies[i - 2] = ie;
                    e *= e;
                    ie *= ie;
                }

                MInt now = 1;
                MInt inow = 1;
                for (var i = 0; i < count2 - 2; i++)
                {
                    _sumE[i] = es[i] * now;
                    _sumIE[i] = ies[i] * inow;
                    now *= ies[i];
                    inow *= es[i];
                }
            }

            private static IEnumerable<MInt> Butterfly(IEnumerable<MInt> items)
            {
                var ret = items.ToArray();
                var h = CeilPower2(ret.Length);
                if (_sumE == null) Initialize();

                for (var ph = 1; ph <= h; ph++)
                {
                    var w = 1 << (ph - 1);
                    var p = 1 << (h - ph);
                    MInt now = 1;
                    for (var s = 0; s < w; s++)
                    {
                        var offset = s << (h - ph + 1);
                        for (var i = 0; i < p; i++)
                        {
                            var l = ret[i + offset];
                            var r = ret[i + offset + p] * now;
                            ret[i + offset] = l + r;
                            ret[i + offset + p] = l - r;
                        }

                        now *= _sumE[BitScanForward(~s)];
                    }
                }

                return ret;
            }

            private static IEnumerable<MInt> ButterflyInverse(IEnumerable<MInt> items)
            {
                var ret = items.ToArray();
                var h = CeilPower2(ret.Length);
                if (_sumIE == null) Initialize();

                for (var ph = h; ph >= 1; ph--)
                {
                    var w = 1 << (ph - 1);
                    var p = 1 << (h - ph);
                    MInt inow = 1;
                    for (var s = 0; s < w; s++)
                    {
                        var offset = s << (h - ph + 1);
                        for (var i = 0; i < p; i++)
                        {
                            var l = ret[i + offset];
                            var r = ret[i + offset + p];
                            ret[i + offset] = l + r;
                            ret[i + offset + p] = (l - r) * inow;
                        }

                        inow *= _sumIE[BitScanForward(~s)];
                    }
                }

                return ret;
            }

            private static int BitScanForward(long n)
            {
                if (n == 0) return 0;
                var x = 0;
                while ((n >> x & 1) == 0) x++;
                return x;
            }

            private static int CeilPower2(int n)
            {
                var x = 0;
                while ((1 << x) < n) x++;
                return x;
            }

            private static long Power(long x, long n, long m)
            {
                if (n < 0) throw new ArgumentException(nameof(n));
                if (m < 1) throw new ArgumentException(nameof(m));
                if (m == 1) return 0;
                uint r = 1;
                var y = (uint)(x % m >= 0 ? x % m : (x + m) % m);
                var um = (uint)m;
                while (n > 1)
                {
                    if (n % 1 == 1) r = r * y % um;
                    y = y * y % um;
                    n >>= 1;
                }

                return r;
            }

            private static int PrimitiveRoot(long m)
            {
                if (_primitiveRoot != 0) return _primitiveRoot;
                switch (m)
                {
                    case 2:
                        return _primitiveRoot = 1;
                    case 167772161:
                    case 469762049:
                    case 998244353:
                        return _primitiveRoot = 3;
                    case 754974721:
                        return _primitiveRoot = 11;
                }

                var divs = new long[20];
                divs[0] = 2;
                var count = 1;
                var x = (m - 1) / 2;
                while (x % 2 == 0) x /= 2;
                for (var i = 3; (long)i * i < x; i += 2)
                {
                    if (x % i != 0) continue;
                    divs[count++] = i;
                    while (x % i == 0) x /= i;
                }

                if (x > 1) divs[count++] = x;
                for (var g = 2; ; g++)
                {
                    var ok = true;
                    for (var i = 0; i < count && ok; i++)
                    {
                        if (Power(g, (m - 1) / divs[i], m) == 1) ok = false;
                    }

                    if (ok) return _primitiveRoot = g;
                }
            }
        }

        public readonly struct MInt
        {
            public long Value { get; }
            public static long Mod { get; private set; } = 998244353;

            public MInt(long data) => Value = (0 <= data ? data : data + Mod) % Mod;
            public static implicit operator long(MInt mint) => mint.Value;
            public static implicit operator int(MInt mint) => (int)mint.Value;
            public static implicit operator MInt(long val) => new MInt(val);
            public static implicit operator MInt(int val) => new MInt(val);
            public static MInt operator +(MInt a, MInt b) => a.Value + b.Value;
            public static MInt operator +(MInt a, long b) => a.Value + b;
            public static MInt operator +(MInt a, int b) => a.Value + b;
            public static MInt operator -(MInt a, MInt b) => a.Value - b.Value;
            public static MInt operator -(MInt a, long b) => a.Value - b;
            public static MInt operator -(MInt a, int b) => a.Value - b;
            public static MInt operator *(MInt a, MInt b) => a.Value * b.Value;
            public static MInt operator *(MInt a, long b) => a.Value * (b % Mod);
            public static MInt operator *(MInt a, int b) => a.Value * (b % Mod);
            public static MInt operator /(MInt a, MInt b) => a * b.Inverse();
            public static MInt operator /(MInt a, long b) => a.Value * Inverse(b);
            public static MInt operator /(MInt a, int b) => a.Value * Inverse(b);
            public static bool operator ==(MInt a, MInt b) => a.Value == b.Value;
            public static bool operator !=(MInt a, MInt b) => a.Value != b.Value;
            public bool Equals(MInt other) => Value == other.Value;
            public override bool Equals(object obj) => obj is MInt other && Equals(other);
            public override int GetHashCode() => Value.GetHashCode();
            public override string ToString() => Value.ToString();

            public MInt Inverse() => Inverse(Value);

            public static MInt Inverse(long a)
            {
                if (a == 0) return 0;
                var p = Mod;
                var (x1, y1, x2, y2) = (1L, 0L, 0L, 1L);
                while (true)
                {
                    if (p == 1) return (x2 % Mod + Mod) % Mod;
                    var div = a / p;
                    x1 -= x2 * div;
                    y1 -= y2 * div;
                    a %= p;
                    if (a == 1) return (x1 % Mod + Mod) % Mod;
                    div = p / a;
                    x2 -= x1 * div;
                    y2 -= y1 * div;
                    p %= a;
                }
            }

            public MInt Power(long n) => Power(Value, n);

            public static MInt Power(MInt x, long n)
            {
                if (n < 0) throw new ArgumentException();
                var r = new MInt(1);
                while (n > 0)
                {
                    if ((n & 1) > 0) r *= x;
                    x *= x;
                    n >>= 1;
                }

                return r;
            }

            public static void SetMod(long m) => Mod = m;
            public static void SetMod998244353() => SetMod(998244353);
            public static void SetMod1000000007() => SetMod(1000000007);
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
