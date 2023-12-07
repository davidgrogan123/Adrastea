using Nightcap.Adrastea.Core;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{

    [TestFixture]
    internal sealed class RegexFileEntryMatcher_tester
    {
        #region setup

        [OneTimeSetUp]
        public void Test_Setup()
        {
            EnforcementCore.Enforcement = true;
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.Exception);
        }
        #endregion

        #region tests

        [Test]
        public void constructor_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => new RegexFileEntryMatcher(null));
        }

        [Test]
        public void constructor_throws_exception_for_invalid_input()
        {
            Assert.Throws<ApplicationException>(() => new RegexFileEntryMatcher("092jkfp0szc[09fjk90[k890`hu"));
        }

        [Test]
        public void constructor_returns_non_null_object()
        {
            Assert.NotNull(new RegexFileEntryMatcher(@""));
            Assert.NotNull(new RegexFileEntryMatcher(@"(\w+)\s+(car)"));
        }

        [Test]
        public void IsMatch_throws_exception_for_null_input()
        {
            var m = new RegexFileEntryMatcher(@"(\w+)\s+(car)");
            Assert.NotNull(m);

            Assert.Throws<CodeContractViolationException>(() => m.IsMatch(null));
        }

        [Test]
        public void IsMatch_returns_false_when_no_match()
        {
            var m = new RegexFileEntryMatcher(@"(\w+)\s+(car)");
            Assert.NotNull(m);

            Assert.IsFalse(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@"")})));
            Assert.IsFalse(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@"One bus red bus blue bus")})));
            Assert.IsFalse(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@"la;oiuhfj89o4hjcloi89jc""';C{AI9;'p30'1[2));")})));
            Assert.IsFalse(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@" car la;oiuhfj89o4hjcloi89jc""';C{AI9;'p30'1[2));")})));
        }

        [Test]
        public void IsMatch_returns_true_when_match()
        {
            var m = new RegexFileEntryMatcher(@"(\w+)\s+(car)");
            Assert.NotNull(m);

            Assert.IsTrue(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext("One car red car blue car")})));
            Assert.IsTrue(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@"l cara;oiuhfj89o4hjcloi89jc""';C{AI9;'p30'1[2));")})));
        }

        #endregion
    }
}
