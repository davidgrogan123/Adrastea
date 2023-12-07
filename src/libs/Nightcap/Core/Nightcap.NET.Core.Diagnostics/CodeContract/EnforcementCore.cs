using Nightcap.Core.Diagnostics.CodeContract.Exceptions;
using NLog;
using System.Diagnostics;

namespace Nightcap.Core.Diagnostics.CodeContract
{
    /// <summary>
    ///  Design by contract pre-condition check enforcement options.
    /// </summary>
    /// <remarks>
    ///  The actions taken by each method in Requires is determined by the 
    ///  state of the following code contract options:
    /// <list type="bullet">
    /// <item>
    /// <term>Enforcement</term>
    /// <description>
    ///  Determines if the code contract is evaluated. If disabled the code 
    ///  contract is not evaluated and no action is taken if it is violated.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Logging</term>
    /// <description>
    ///  If enabled, when a code contract is violated a message will be
    ///  logged. This option may be combined with Exception or Termination.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Exception</term>
    /// <description>
    ///  If enabled, when a code contract is violated an exception will be
    ///  thrown. This option may be combined with Logging but is mutually
    ///  exclusive to Termination.
    /// </description>
    /// </item>
    /// <item>
    /// <term>Termination</term>
    /// <description>
    ///  If enabled, when a code contract is violated the process will
    ///  terminate with an exit code of <c>1</c>. This option may be
    ///  combined with Logging but is mutually exclusive to Exception.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public static class EnforcementCore
    {
        #region types

        /// <summary>
        ///  Specifies the action to take when the code contract is violated
        ///  and enforcement is enabled.
        /// </summary>
        [Flags]
        public enum ViolationReportAction
        {
            /// <summary>
            ///  No action is taken if the code contract is violated. 
            /// </summary>
            Ignore = 0x00,
            /// <summary>
            ///  The code contract violation is logged.
            /// </summary>
            LogThenIgnore = 0x01,
            /// <summary>
            ///  An exception is thrown when the code contract is violated.
            /// </summary>
            Exception = 0x10,
            /// <summary>
            ///  The process is terminated if the code contract is violated.
            /// </summary>
            Terminate = 0x20,
            /// <summary>
            ///  The code contract violation is logged and an exception is generated.
            /// </summary>
            LogAndReportByException = LogThenIgnore | Exception,
            /// <summary>
            ///  The code contract violation is logged and an the process is terminated.
            /// </summary>
            LogAndGaranteedTermination = LogThenIgnore | Terminate,
        }
        #endregion

        #region constants

        private static class Constants
        {
            internal static class StatusCode
            {
                internal const int Success = 0;
                internal const int Failure = 1;
                internal const int Minimum = 1;
                internal const int Maximum = 255;
            }

            internal static class FailureMessage
            {
                internal const string InvalidStatusCode_1 = "Status code {0} "
                    + "invalid; must be between 1 and 255; set to default 1";
            }
        }
        #endregion

        #region fields

        private static int sm_violationExitCode;
        private static bool sm_enforcingContracts;

        private static ViolationReportAction sm_reporterType;

        private static Logger sm_logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region properties

        /// <summary>
        ///  Indicates the exit code to be used when terminating the process
        ///  due to a code contract violation. When settings the exit code 
        ///  it must be in the range of 1-255, values out of this range will 
        ///  cause the exit code to be set to the default, one (1).
        /// </summary>
        public static int ViolationExitCode
        {
            get
            {
                return sm_violationExitCode;
            }

            set
            {
                if (Constants.StatusCode.Minimum > value
                    || Constants.StatusCode.Maximum < value
                )
                {
                    sm_violationExitCode = Constants.StatusCode.Failure;

                    sm_logger.Warn(Constants.FailureMessage.InvalidStatusCode_1, value);
                }
                else
                {
                    sm_violationExitCode = value;
                }
            }
        }

        /// <summary>
        ///  Indicates if code contract enforcement is enabled.
        /// </summary>
        public static bool Enforcement
        {
            get
            {
                return sm_enforcingContracts;
            }

            set
            {
                sm_enforcingContracts = value;
            }
        }

        /// <summary>
        ///  Indicates that no reporter actions have been set for code 
        ///  contract violations.
        /// </summary>
        public static bool ContractFailuresAreIgnored
        {
            get
            {
                return ViolationReportAction.Ignore == sm_reporterType;
            }
        }

        /// <summary>
        ///  Indicates a logging message will be generated if the code
        ///  contract is violated. This action will only be taken if code
        ///  contract enforcement is enabled.
        /// </summary>
        public static bool ContractFailuresAreLogged
        {
            get
            {
                return 0 != (sm_reporterType & ViolationReportAction.LogThenIgnore);
            }
        }

        /// <summary>
        ///  Indicates if an exception will be thrown if a code contract is
        ///  violated. This action will only be taken if code contract
        ///  enforcement is enabled.
        /// </summary>
        public static bool ContractFailuresAreReportedByException
        {
            get
            {
                return 0 != (sm_reporterType & ViolationReportAction.Exception);
            }
        }

        /// <summary>
        ///  Indicates if the process will terminate if a code contract is
        ///  violated. This action will only be taken if code contract
        ///  enforcement is enabled.
        /// </summary>
        public static bool ContractFailuresCauseTermination
        {
            get
            {
                return 0 != (sm_reporterType & ViolationReportAction.Terminate);
            }
        }
        #endregion

        #region accessors

        /// <summary>
        ///  Configures the reporter type to define the actions to be taken 
        ///  when a code contract is violated.
        /// </summary>
        /// <param name="reporterType">
        ///  Defines the actions to be taken.
        /// </param>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
        ///  Thrown if <paramref name="reporterType"/> has an invalid value.
        /// </exception>
        public static void ConfigureReporterType(ViolationReportAction reporterType)
        {
            if (Enum.IsDefined(typeof(ViolationReportAction), reporterType))
            {
                sm_reporterType = reporterType;
            }
            else
            {
                throw new System.ComponentModel.InvalidEnumArgumentException(
                    "reporterType"
                    , (int)reporterType
                    , typeof(ViolationReportAction)
                );
            }
        }
        #endregion

        #region constructor

        /// <summary>
        ///  Constructor. Default options are all disabled. In debug,
        ///  enforcement, logging, and exceptions are enabled. 
        /// </summary>
        static EnforcementCore()
        {
            sm_enforcingContracts = false;
            sm_reporterType = ViolationReportAction.Ignore;
            sm_violationExitCode = Constants.StatusCode.Failure;

            DebugSettings();
        }
        #endregion

        #region internal methods

        internal static string DeriveCallContextInfo(int frame)
        {
            return new StackTrace().GetFrame(frame + 1).GetMethod().Name;
        }

        internal static void TerminateIfRequired()
        {
            if (0 != (ViolationReportAction.Terminate & sm_reporterType))
            {
                Environment.Exit(sm_violationExitCode);
            }
        }

        internal static void Dispatch(string message, ViolationType cause)
        {
            if (0 != (ViolationReportAction.LogThenIgnore & sm_reporterType))
            {
                sm_logger.Fatal(message);
            }

            if (0 != (ViolationReportAction.Exception & sm_reporterType))
            {
                throw new CodeContractViolationException(message, cause);
            }
        }
        #endregion

        #region private methods

        [Conditional("DEBUG")]
        private static void DebugSettings()
        {
            sm_enforcingContracts = true;
            sm_reporterType = ViolationReportAction.LogAndReportByException;
        }
        #endregion
    }
}
