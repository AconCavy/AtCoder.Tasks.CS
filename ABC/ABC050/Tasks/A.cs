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
            var S = Console.ReadLine().Trim().Split(' ');
            var (A, B) = (int.Parse(S[0]), int.Parse(S[2]));
            var op = S[1];
            var answer = op == "+" ? A + B : A - B;
            Console.WriteLine(answer);
        }
    }
}
