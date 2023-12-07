using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;
using Nightcap.NET.Core.IO;

namespace Test.Unit.Nightcap.Core.IO
{
    [TestFixture]
    internal sealed class SharingViolationException_tester
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
        public void Test_Constructor_returns_non_null_object()
        {
            Assert.IsNotNull(new SharingViolationException());
            Assert.IsNotNull(new SharingViolationException("hello"));
            Assert.IsNotNull(new SharingViolationException("hello", new Exception()));
        }

        [Test]
        public void Test_Constructor_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => new SharingViolationException(null));
            Assert.Throws<CodeContractViolationException>(() => new SharingViolationException("hello", null));
            Assert.Throws<CodeContractViolationException>(() => new SharingViolationException(null, null));
        }

        [Test]
        public void Test_Constructor_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => new SharingViolationException(""));
            Assert.Throws<CodeContractViolationException>(() => new SharingViolationException("", new Exception()));
        }
        #endregion
    }
}
