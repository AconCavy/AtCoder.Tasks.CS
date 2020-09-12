using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tasks
{
    public class B
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
            var G = new int[3][].Select(x => Scanner.ScanEnumerable<int>().ToArray()).ToArray();
            var N = Scanner.Scan<int>();
            var B = new bool[3][].Select(x => new bool[3]).ToArray();
            for (var k = 0; k < N; k++)
            {
                var b = Scanner.Scan<int>();
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 3; j++)
                    {
                        if (G[i][j] == b) B[i][j] = true;
                    }
                }
            }

            var answer = false;
            for (var i = 0; i < 3; i++)
            {
                if (B[i][0] && B[i][1] && B[i][2]) answer = true;
            }
            for (var i = 0; i < 3; i++)
            {
                if (B[0][i] && B[1][i] && B[2][i]) answer = true;
            }
            if (B[0][0] && B[1][1] && B[2][2]) answer = true;
            if (B[0][2] && B[1][1] && B[2][0]) answer = true;

            Console.WriteLine(answer ? "Yes" : "No");
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
