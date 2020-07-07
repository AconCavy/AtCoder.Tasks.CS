using Microsoft.VisualStudio.TestTools.UnitTesting;
using D;

namespace Tests
{
    [TestClass]
    public class TestD
    {
        [TestMethod]
        public void TestMethod1()
        {
            var input = @"3
1 2 1 3";
            var output = @"3
5
4
1";
            Tester.InOutTest(() => Program.Solve(), input, output);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var input = @"1
1 1";
            var output = @"1
1";
            Tester.InOutTest(() => Program.Solve(), input, output);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var input = @"32
29 19 7 10 26 32 27 4 11 20 2 8 16 23 5 14 6 12 17 22 18 30 28 24 15 1 25 3 13 21 19 31 9";
            var output = @"32
525
5453
40919
237336
1107568
4272048
13884156
38567100
92561040
193536720
354817320
573166440
818809200
37158313
166803103
166803103
37158313
818809200
573166440
354817320
193536720
92561040
38567100
13884156
4272048
1107568
237336
40920
5456
528
33
1";
            Tester.InOutTest(() => Program.Solve(), input, output);
        }
    }
}
