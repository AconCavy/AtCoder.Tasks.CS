﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tasks
{
    public class A
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
            var ABC = Console.ReadLine().Trim().Split(' ').Select(int.Parse).ToArray();
            var (A, B, C) = (ABC[0], ABC[1], ABC[2]);
            var answer = false;
            answer |= A + B == C;
            answer |= A + C == B;
            answer |= B + C == A;
            Console.WriteLine(answer ? "Yes" : "No");
        }
    }
}
