using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class BTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const string input = @"4
4 2 3 1
2 3 2 4";
            const string output = @"12";
            Tester.InOutTest(() => Tasks.B.Solve(), input, output);
        }

        [TestMethod]
        public void TestMethod2()
        {
            const string input = @"10
866111664 844917655 383133839 353498483 472381277 550309930 378371075 304570952 955719384 705445072
178537096 218662351 231371336 865935868 579910117 62731178 681212831 16537461 267238505 318106937";
            const string output = @"6629738472";
            Tester.InOutTest(() => Tasks.B.Solve(), input, output);
        }
    }
}
