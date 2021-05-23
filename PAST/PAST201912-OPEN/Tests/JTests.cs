using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class JTests
    {
        const int TimeLimit = 2000;
        const double RelativeError = 1e-9;

        [TestMethod, Timeout(TimeLimit)]
        public void Test1()
        {
            const string input = @"5 6
9 9 9 9 1 0
9 9 9 9 1 9
9 9 9 1 1 1
9 1 1 1 9 1
0 1 9 9 9 0
";
            const string output = @"10
";
            Tester.InOutTest(Tasks.J.Solve, input, output);
        }

        [TestMethod, Timeout(TimeLimit)]
        public void Test2()
        {
            const string input = @"10 10
1 2 265 1544 0 1548 4334 9846 58 0
21 0 50 44 2 388 5 0 0 4
170 0 2 1 54 1379 50 3 41 0
310 0 1 0 2163 0 226 26 3 12
151 33 0 9 0 0 0 36 365 2286
0 3 12 3 9 317 645 100 21 4
52 1 569 0 144 0 6 202 25 0
8869 19 2058 1948 1252 1002 7 1750 0 5
0 3 8 29 2 4403 0 0 0 5
0 17 93 9367 159 6 1 216 0 0
";
            const string output = @"246
";
            Tester.InOutTest(Tasks.J.Solve, input, output);
        }
    }
}