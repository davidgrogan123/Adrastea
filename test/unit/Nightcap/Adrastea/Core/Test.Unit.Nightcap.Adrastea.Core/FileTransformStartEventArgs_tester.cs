using Nightcap.Adrastea.Core;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class FileTransformStartEventArgs_tester
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
        public void FileTransformStartEventArgs_creates_object_correctly()
        {
            string  filePath = "filePath";

            FileTransformStartEventArgs eventArgs = new FileTransformStartEventArgs(filePath);
            Assert.NotNull(eventArgs, "Constructor did not create object");
            Assert.AreEqual(filePath, eventArgs.FilePath, "'FilePath' not correctly set");
        }

        [Test]
        public void FileTransformStartEventArgs_constructor_enforces_code_contract()
        {
            string  filePath = "filePath";

            Assert.DoesNotThrow(
                () => new FileTransformStartEventArgs(filePath)
                , "Should not throw exception during construction"
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileTransformStartEventArgs(null)
                , "Should throw exception for null 'filePath'"
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileTransformStartEventArgs(" ")
                , "Should throw exception for whitespace 'filePath'"
            );
        }
        #endregion
    }
}
