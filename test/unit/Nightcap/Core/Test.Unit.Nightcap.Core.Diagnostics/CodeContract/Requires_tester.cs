using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Core.Diagnostics.CodeContract
{
    #region tests

    [TestFixture]
    internal sealed class RequiresTestExceptions
    {
        [SetUp]
        public void Test_Requires_Exception_Setup()
        {
            EnforcementCore.Enforcement = true;
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.Exception);
        }

        [Test]
        public void Test_ThatArgumentIsNotNull_throws_exception_when_argument_is_null()
        {
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNull(null, "someArg"));

            object j = null;
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNull(j, "j"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNull_doesnt_throw_exception_when_argument_is_not_null()
        {
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull("", "some Arg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(" ", "some Arg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(5, "someArg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(new object(), "some Arg"));

            var arg = new List();
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(arg, "arg"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNullOrWhiteSpace_throws_exception_when_argument_is_null()
        {
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(null, "someArg"));

            string j = null;
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(j, "j"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNullOrWhiteSpace_throws_exception_when_argument_is_whitespace()
        {
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("", "some Arg"));
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(" ", "some Arg"));
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(" \t ", "some Arg"));
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(" \t  \n ", "some Arg"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNullOrWhiteSpace_does_not_throw_exception_when_argument_is_not_null_or_whitespace()
        {
            Assert.That(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("5", "someArg"), Throws.Nothing);
            Assert.That(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("   7  ", "some Arg"), Throws.Nothing);
        }

        [Test]
        public void Test_ThatArgument_throws_exception_when_predicate_is_false()
        {
            int x = 1;
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgument(x > 1, "x", "message"), "x must be greater than 1");
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgument(false, "x", "message"), "false");
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgument(!true, "x", "message"), "not true");
        }

        [Test]
        public void Test_ThatArgument_throws_exception_when_predicate_delegate_is_false()
        {
            int x = 1;

            Predicate predicate = () => x > 1;
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgument(predicate, "x", "message"), "x must be greater than 1");

            predicate = () => false;
            Assert.Throws<CodeContractViolationException>(() => Requires.ThatArgument(predicate, "x", "message"), "false");
        }

        [Test]
        public void Test_ThatArgument_does_not_throw_exception_when_predicate_is_true()
        {
            int x = 1;
            Assert.DoesNotThrow(() => Requires.ThatArgument(x > 0, "x", "x must be greater than 0"));
            Assert.DoesNotThrow(() => Requires.ThatArgument(x > 0, "   x    ", "x must be greater than 0"));
            Assert.DoesNotThrow(() => Requires.ThatArgument(x > 0, "   x    ", "    \n  c \t  "));
        }

        [Test]
        public void Test_ThatArgument_does_not_throw_exception_when_predicate_delegate_is_true()
        {
            int x = 1;

            Predicate predicate = () => x > 0;
            Assert.DoesNotThrow(() => Requires.ThatArgument(predicate, "x", "message"), "x must be greater than 0");

            predicate = () => true;
            Assert.DoesNotThrow(() => Requires.ThatArgument(predicate, "   x    ", "message"), "true");
        }

        [Test]
        public void Test_That_throws_exception_when_predicate_is_false()
        {
            int x = 1;
            Assert.Throws<CodeContractViolationException>(() => Requires.That(x > 1, "message"), "x is greater than 1");
            Assert.Throws<CodeContractViolationException>(() => Requires.That(false, "message"), "predicate false");
        }

        [Test]
        public void Test_That_throws_exception_when_predicate_delegate_is_false()
        {
            int x = 1;

            Predicate predicate = () => x > 1;
            Assert.Throws<CodeContractViolationException>(() => Requires.That(predicate, "message"), "x must be greater than 1");

            predicate = () => false;
            Assert.Throws<CodeContractViolationException>(() => Requires.That(predicate, "message"), "false");
        }

        [Test]
        public void Test_That_does_not_throw_exception_when_predicate_is_true()
        {
            int x = 1;
            Assert.DoesNotThrow(() => Requires.That(x > 0, "x must be greater than 0"));
            Assert.DoesNotThrow(() => Requires.That(x > 0, "x must be greater than 0"));
            Assert.DoesNotThrow(() => Requires.That(x > 0, "    \n  c \t  "));
        }

        [Test]
        public void Test_That_does_not_throw_exception_when_predicate_delegate_is_true()
        {
            int x = 1;

            Predicate predicate = () => x > 0;
            Assert.DoesNotThrow(() => Requires.That(predicate, "message"), "x must be greater than 0");

            predicate = () => true;
            Assert.DoesNotThrow(() => Requires.That(predicate, "message"), "true");
        }
    }

    [TestFixture]
    internal sealed class RequiresTestDisabled
    {
        [SetUp]
        public void Test_Requires_Disabled_Setup()
        {
            EnforcementCore.Enforcement = false;
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.Exception);
        }

        [Test]
        public void Test_ThatArgumentIsNotNull_does_not_throw_or_exit_when_argument_is_null()
        {
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(null, "someArg"));

            object j = null;
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNull(j, "j"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNullOrWhiteSpace_does_not_throw_or_exit_when_argument_is_null()
        {
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(null, "someArg"));
        }

        [Test]
        public void Test_ThatArgumentIsNotNullOrWhiteSpace_does_not_throw_or_exit_when_argument_is_whitespace()
        {
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("", "someArg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace(" ", "someArg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("\t", "someArg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("\n", "someArg"));
            Assert.DoesNotThrow(() => Requires.ThatArgumentIsNotNullOrWhiteSpace("   \t   \n   ", "someArg"));
        }

        [Test]
        public void Test_ThatArgument_does_not_throw_or_exit_when_pedicate_is_false()
        {
            int x = 1;
            Assert.DoesNotThrow(() => Requires.ThatArgument(false, "someArg", "someMessage"));
            Assert.DoesNotThrow(() => Requires.ThatArgument(x < 1, "someArg", "someMessage"));
        }
    }
    #endregion
}
