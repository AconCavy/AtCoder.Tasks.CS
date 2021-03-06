using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Tasks
{
    public class C
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
            var S1 = Scanner.Scan<string>();
            var S2 = Scanner.Scan<string>();
            var S3 = Scanner.Scan<string>();
            var c1 = new int[26];
            var c2 = new int[26];
            var c3 = new int[26];
            foreach (var c in S1) c1[c - 'A']++;
            foreach (var c in S2) c2[c - 'A']++;
            foreach (var c in S3) c3[c - 'A']++;

            var N = S1.Length / 2;
            var (min, max) = (0, 0);
            for (var i = 0; i < 26; i++)
            {
                if (c1[i] + c2[i] < c3[i])
                {
                    Console.WriteLine("NO");
                    return;
                }

                min += Math.Max(0, c3[i] - c2[i]);
                max += Math.Min(c1[i], c3[i]);
            }

            var answer = min <= N && N <= max;
            Console.WriteLine(answer ? "YES" : "NO");
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
