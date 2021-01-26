using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class ATests
    {
        const int TimeLimit = 2000;
        const double RelativeError = 1e-9;

        [TestMethod, Timeout(TimeLimit)]
        public void Test1()
        {
            const string input = @"3
3 2 20
1 4 1
";
            const string output = @"3
12
20
";
            Tester.InOutTest(Tasks.A.Solve, input, output);
        }

        [TestMethod, Timeout(TimeLimit)]
        public void Test2()
        {
            const string input = @"20
715806713 926832846 890153850 433619693 890169631 501757984 778692206 816865414 50442173 522507343 546693304 851035714 299040991 474850872 133255173 905287070 763360978 327459319 193289538 140803416
974365976 488724815 821047998 371238977 256373343 218153590 546189624 322430037 131351929 768434809 253508808 935670831 251537597 834352123 337485668 272645651 61421502 439773410 621070911 578006919
";
            const string output = @"697457706539596888
697457706539596888
760974252688942308
760974252688942308
760974252688942308
760974252688942308
760974252688942308
760974252688942308
760974252688942308
760974252688942308
760974252688942308
867210459214915026
867210459214915026
867210459214915026
867210459214915026
867210459214915026
867210459214915026
867210459214915026
867210459214915026
867210459214915026
";
            Tester.InOutTest(Tasks.A.Solve, input, output);
        }
    }
}