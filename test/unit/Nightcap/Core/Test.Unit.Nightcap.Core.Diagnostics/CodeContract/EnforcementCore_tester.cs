using global::Nightcap.Core.Diagnostics.CodeContract;

namespace Test.Unit.Nightcap.Core.Diagnostics.CodeContract
{
    #region default value

    internal static class EnforcementCoreDefaults
    {
        public static bool Enforcement { get; set; }
        public static bool Logging { get; set; }
        public static bool Exception { get; set; }
        public static bool Termination { get; set; }
    }
    #endregion

    #region setup

    [SetUpFixture]
    public sealed class EnforcementCoreTestSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            EnforcementCoreDefaults.Enforcement = EnforcementCore.Enforcement;
            EnforcementCoreDefaults.Logging = EnforcementCore.ContractFailuresAreLogged;
            EnforcementCoreDefaults.Exception = EnforcementCore.ContractFailuresAreReportedByException;
            EnforcementCoreDefaults.Termination = EnforcementCore.ContractFailuresCauseTermination;
        }
    }
    #endregion

    #region tests

    [TestFixture]
    internal sealed class EnforcementCoreTestDefaults
    {
        [Test]
        public void Test_EnforcementCore_has_correct_defaults()
        {
#if DEBUG
            Assert.IsTrue(EnforcementCoreDefaults.Enforcement, "Enforcement True in debug");
            Assert.IsTrue(EnforcementCoreDefaults.Logging, "Logging True in debug");
            Assert.IsTrue(EnforcementCoreDefaults.Exception, "Exception True in debug");
            Assert.IsFalse(EnforcementCoreDefaults.Termination, "Termination False in debug");
#else
            Assert.IsFalse(EnforcementCoreDefaults.Enforcement, "Enforcement False in Release");
            Assert.IsFalse(EnforcementCoreDefaults.Logging, "Logging False in Release");
            Assert.IsFalse(EnforcementCoreDefaults.Exception, "Exception False in Release");
            Assert.IsFalse(EnforcementCoreDefaults.Termination, "Termination False in Release");
#endif
        }
    }

    [TestFixture]
    internal sealed class EnforcementCoreTestOptions
    {
        [SetUp]
        public void Test_EnforcementCore_Options_Setup()
        {
            // Disable all
            Assert.DoesNotThrow(
                () => EnforcementCore.Enforcement = false
                , "Disable Enforcement threw exception"
            );
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Ignore
                    )
                , "Configure no action threw exception"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_enforcement()
        {
            Assert.DoesNotThrow(
                () => EnforcementCore.Enforcement = true
                , "Enable Enforcement threw exception"
            );
            Assert.True(
                EnforcementCore.Enforcement
                , "Failed to enable Enforcement"
            );
            Assert.DoesNotThrow(
                () => EnforcementCore.Enforcement = false
                , "Disable Enforcement threw exception"
            );
            Assert.False(
                EnforcementCore.Enforcement
                , "Failed to disable Enforcement"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_logging()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.LogThenIgnore
                    )
                , "Configure logging threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreLogged
                , "Failed to enable logging"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_exception()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Exception
                    )
                , "Configure exception threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreReportedByException
                , "Failed to enable exception"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_termination()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Terminate
                    )
                , "Configure termination threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresCauseTermination
                , "Failed to enable termination"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_log_and_exception()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.LogAndReportByException
                    )
                , "Configure exception threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreLogged
                , "Failed to enable logging"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreReportedByException
                , "Failed to enable exception"
            );
        }

        [Test]
        public void Test_EnforcementCore_can_change_log_and_termination()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.LogAndGaranteedTermination
                    )
                , "Configure termination threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreLogged
                , "Failed to enable logging"
            );
            Assert.True(
                EnforcementCore.ContractFailuresCauseTermination
                , "Failed to enable termination"
            );
        }

        [Test]
        public void Test_EnforcementCore_exception_and_termination_mutually_exclusive()
        {
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Exception
                    )
                , "Configure exception threw exception"
            );
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Terminate
                    )
                , "Configure termination threw exception"
            );
            Assert.False(
                EnforcementCore.ContractFailuresAreReportedByException
                , "Exception option should have been overridden"
            );
            Assert.True(
                EnforcementCore.ContractFailuresCauseTermination
                , "Failed to enable termination"
            );

            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Terminate
                    )
                , "Configure termination threw exception"
            );
            Assert.DoesNotThrow(
                () =>
                    EnforcementCore.ConfigureReporterType(
                        EnforcementCore.ViolationReportAction.Exception
                    )
                , "Configure exception threw exception"
            );
            Assert.True(
                EnforcementCore.ContractFailuresAreReportedByException
                , "Failed to enable exception"
            );
            Assert.False(
                EnforcementCore.ContractFailuresCauseTermination
                , "Termination option should have been overridden"
            );

            EnforcementCore.ViolationReportAction ExAndTerm =
                EnforcementCore.ViolationReportAction.Exception
                | EnforcementCore.ViolationReportAction.Terminate;

            Assert.Throws<System.ComponentModel.InvalidEnumArgumentException>(
                () => EnforcementCore.ConfigureReporterType(ExAndTerm)
                , "No exception thrown when enabling exception and termination"
            );
        }

        [Test]
        public void Test_EnforcementCore_enforcement_concurrent_to_options()
        {
            EnforcementCore.Enforcement = true;
            Assert.True(EnforcementCore.Enforcement, "Failed to enable Enforcement");

            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.LogThenIgnore);
            Assert.True(EnforcementCore.ContractFailuresAreLogged, "Failed to enable logging");
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.Exception);
            Assert.True(EnforcementCore.ContractFailuresAreReportedByException, "Failed to enable exception");
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.LogAndReportByException);
            Assert.True(EnforcementCore.ContractFailuresAreReportedByException, "Failed to enable logging and exception");

            EnforcementCore.Enforcement = false;
            Assert.False(EnforcementCore.Enforcement, "Failed to disable Enforcement");
            Assert.True(EnforcementCore.ContractFailuresAreLogged, "Logging was disabled by enforcement");
            Assert.True(EnforcementCore.ContractFailuresAreReportedByException, "Exception was disabled by enforcement");
        }

        [Test]
        public void Test_EnforcementCore_ViolationExitCode_can_be_set([Values(1, 2, 254, 255)] int exitCode)
        {
            Assert.DoesNotThrow(
                () => EnforcementCore.ViolationExitCode = exitCode
                , "Setting ViolationExitCode to " + exitCode + " threw exception"
            );

            Assert.That(
                EnforcementCore.ViolationExitCode
                , Is.EqualTo(exitCode)
                , "Failed to set exit code to " + exitCode
            );
        }

        [Test]
        public void Test_EnforcementCore_ViolationExitCode_set_to_one_when_invalid([Values(0, -1, 256)] int exitCode)
        {
            Assert.DoesNotThrow(
                () => EnforcementCore.ViolationExitCode = exitCode
                , "Setting ViolationExitCode to " + exitCode + " threw exception"
            );

            Assert.That(
                EnforcementCore.ViolationExitCode
                , Is.EqualTo(1)
                , "Exit code not set to default"
            );
        }
    }
    #endregion
}