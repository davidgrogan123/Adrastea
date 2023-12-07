namespace Nightcap.Core.Diagnostics.CodeContract
{
    /// <summary>
    ///  Design by contract pre-condition checks.
    /// </summary>
    /// <remarks>
    ///  This class contains only the code contract enforcement. Configuration
    ///  of code contract enforcement is done by EnforcementCore. All new 
    ///  code contract methods should follow the same layout as existing.
    ///  
    ///  The actions taken by each method is determined by the state of the
    ///  following code contract options in EnforcementCore:
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
    ///
    ///  This is a lightweight replacement for some functionality in the
    ///  Microsoft .NET Code Contracts library.
    ///
    ///  (We are avoiding Microsoft Code Contracts at the moment because it is
    ///  cumbersome and doesn't receive active support.)
    /// </remarks>
    public static class Requires
    {
        #region constants

        private static class Constants
        {
            internal static class FailureMessage
            {
                internal const string ArgumentIsNull_2 =
                    "Argument '{0}' from method '{1}' is null";
                internal const string ArgumentIsNullOrWhitespace_2 =
                    "Argument '{0}' from method {1} is null or whitespace";
                internal const string ArgumentPredicateFailed_3 =
                    "Argument '{0}' from method {0} failed the predicate " +
                    "with message {2}";
                internal const string PredicateFailed_2 =
                    "Predicate from method {0} failed with message {1}";
            }
        }
        #endregion

        #region methods

        /// <summary>
        ///  Check for a <c>null</c> argument.
        /// </summary>
        /// <param name="arg">
        ///  The value of the parameter that is being checked.
        /// </param>
        /// <param name="argName">
        ///  The name of the parameter that is being checked.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when <paramref name="arg"/> is <c>null</c> and the
        ///  exception option is enabled in EnforcementCore.
        /// </exception>
        public static void ThatArgumentIsNotNull(
              object arg
            , string argName
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if (null != arg)
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.ArgumentIsNull_2
                        , argName
                        , context
                    )
                    , ViolationType.ArgumentWasNull
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }

        /// <summary>
        ///  Check for a <c>null</c> or whitespace string argument.
        /// </summary>
        /// <param name="arg">
        ///  The value of the parameter that is being checked.
        /// </param>
        /// <param name="argName">
        ///  The name of the parameter that is being checked.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when <paramref name="arg"/> is <c>null</c> or contains
        ///  only whitespace and the exception option is enabled in 
        ///  EnforcementCore.
        /// </exception>
        public static void ThatArgumentIsNotNullOrWhiteSpace(
              string arg
            , string argName
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if ((null != arg) && (0 != arg.Trim().Length))
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.ArgumentIsNullOrWhitespace_2
                        , argName
                        , context
                    )
                    , ViolationType.ArgumentWasNullOrWhitespace
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }

        /// <summary>
        ///  Check argument predicate result.
        /// </summary>
        /// <param name="predicate">
        ///  The predicate result that will be checked.
        /// </param>
        /// <param name="argName">
        ///  The name of the parameter that is being checked.
        /// </param>
        /// <param name="message">
        ///  The error message that explains the reason for any exception.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when the predicate result is <c>false</c> and the
        ///  exception option is enabled in EnforcementCore.
        /// </exception>
        public static void ThatArgument(
              bool predicate
            , string argName
            , string message
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if (predicate)
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.ArgumentPredicateFailed_3
                        , argName
                        , context
                        , message
                    )
                    , ViolationType.ArgumentPredicateWasFalse
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }

        /// <summary>
        ///  Check argument with a predicate delegate.
        /// </summary>
        /// <param name="predicate">
        ///  The predicate delegate that will be checked.
        /// </param>
        /// <param name="argName">
        ///  The name of the parameter that is being checked.
        /// </param>
        /// <param name="message">
        ///  The error message that explains the reason for any exception.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when the predicate evaluates to <c>false</c> and the
        ///  exception option is enabled in EnforcementCore.
        /// </exception>
        /// <example>
        ///  This sample shows how to define a <see cref="Predicate"/> 
        ///  delegate for code contract enforcement using the 
        ///  <see cref="Requires.ThatArgument(Predicate,string,string)"/> method.
        /// <code>
        ///  Predicate predicate = () => x > 0;
        ///  Requires.ThatArgument(predicate, "x", "must be greater than zero");
        /// </code>
        /// </example>
        public static void ThatArgument(
              Predicate predicate
            , string argName
            , string message
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if (predicate.Invoke())
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.ArgumentPredicateFailed_3
                        , argName
                        , context
                        , message
                    )
                    , ViolationType.ArgumentPredicateWasFalse
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }

        /// <summary>
        ///  Check a predicate result.
        /// </summary>
        /// <param name="predicate">
        ///  The predicate result that will be checked.
        /// </param>
        /// <param name="message">
        ///  The error message that explains the reason for any exception.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when the predicate evaluates to <c>false</c> and the
        ///  exception option is enabled in EnforcementCore.
        /// </exception>
        public static void That(
              bool predicate
            , string message
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if (predicate)
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.PredicateFailed_2
                        , context
                        , message
                    )
                    , ViolationType.PredicateWasFalse
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }

        /// <summary>
        ///  Check a predicate delegate.
        /// </summary>
        /// <param name="predicate">
        ///  The predicate delegate that will be checked.
        /// </param>
        /// <param name="message">
        ///  The error message that explains the reason for any exception.
        /// </param>
        /// <exception cref="Exceptions.CodeContractViolationException">
        ///  Thrown when the predicate evaluates to <c>false</c> and the
        ///  exception option is enabled in EnforcementCore.
        /// </exception>
        /// <example>
        ///  This sample shows how to define a <see cref="Predicate"/> 
        ///  delegate for code contract enforcement using the 
        ///  <see cref="Requires.That(Predicate,string)"/> method.
        /// <code>
        ///  Predicate predicate = () => x > 0;
        ///  Requires.That(predicate, "x must be greater than zero");
        /// </code>
        /// </example>
        public static void That(
              Predicate predicate
            , string message
        )
        {
            if (!EnforcementCore.Enforcement)
            {
                return;
            }

            if (EnforcementCore.ContractFailuresAreIgnored)
            {
                return;
            }

            try
            {
                if (predicate.Invoke())
                {
                    return;
                }

                string context = EnforcementCore.DeriveCallContextInfo(1);

                EnforcementCore.Dispatch(
                    string.Format(
                        Constants.FailureMessage.PredicateFailed_2
                        , context
                        , message
                    )
                    , ViolationType.PredicateWasFalse
                );
            }
            finally
            {
                EnforcementCore.TerminateIfRequired();
            }
        }
        #endregion
    }
}
