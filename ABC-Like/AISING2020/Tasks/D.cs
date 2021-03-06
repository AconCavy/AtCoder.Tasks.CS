using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Tasks
{
    public class D
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
            var N = Scanner.Scan<int>();
            var S = Scanner.Scan<string>();

            var memo = new int[N + 1];
            for (var i = 1; i < N; i++) memo[i] = i % BitOperations.PopCount((uint)i);
            var count = new int[N + 1];
            for (var i = 1; i < N; i++) count[i] = count[memo[i]] + 1;

            var pc = S.Count(x => x == '1');
            var is0 = pc == 0;
            var is1 = pc == 1;
            var (pc1, pc2) = (pc + 1, pc - 1);
            var k1 = new int[N];
            var k2 = new int[N];
            var (m1, m2) = (0, 0);
            k1[0] = k2[0] = 1;
            for (var i = 0; i < N; i++)
            {
                if (i - 1 >= 0) k1[i] = k1[i - 1] * 2 % pc1;
                m1 = (m1 * 2 + (S[i] - '0')) % pc1;
            }

            if (!is0 && !is1)
            {
                for (var i = 0; i < N; i++)
                {
                    if (i - 1 >= 0) k2[i] = k2[i - 1] * 2 % pc2;
                    m2 = (m2 * 2 + (S[i] - '0')) % pc2;
                }
            }

            for (var i = 0; i < N; i++)
            {
                if (is1 && S[i] == '1') { Console.WriteLine(0); continue; }
                var m = S[i] == '0' ?
                ((m1 + k1[N - i - 1]) % pc1 + pc1) % pc1 :
                ((m2 - k2[N - i - 1]) % pc2 + pc2) % pc2;
                Console.WriteLine(count[m] + 1);
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
