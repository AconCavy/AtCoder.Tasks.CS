using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class DTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var input = @"5 3
1 2 4 7 8
2
1 5
1 2";
            var output = @"4
2";
            Tester.InOutTest(() => Tasks.D.Solve(), input, output);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var input = @"15 220492538
4452279 12864090 23146757 31318558 133073771 141315707 263239555 350278176 401243954 418305779 450172439 560311491 625900495 626194585 891960194
5
6 14
1 8
1 13
7 12
4 12";
            var output = @"4
6
11
2
3";
            Tester.InOutTest(() => Tasks.D.Solve(), input, output);
        }
    }
}
