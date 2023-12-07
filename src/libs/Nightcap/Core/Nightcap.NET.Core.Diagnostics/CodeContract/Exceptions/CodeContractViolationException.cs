namespace Nightcap.Core.Diagnostics.CodeContract.Exceptions
{
    /// <summary>
    ///  Exception thrown when a code contract is violated.
    /// </summary>
    public class CodeContractViolationException : Exception
    {
        #region fields

        private readonly ViolationType m_cause;
        #endregion

        #region constructors

        /// <summary>
        ///  Initializes a with an error message.
        /// </summary>
        /// <param name="message">
        ///  The error message.
        /// </param>
        /// <param name="cause">
        ///  The cause of the code contract violation.
        /// </param>
        public CodeContractViolationException(
            string message
            , ViolationType cause
        )
            : base(message)
        {
            m_cause = cause;
        }

        /// <summary>
        ///  Initializes with an error message and inner exception.
        /// </summary>
        /// <param name="message">
        ///  The error message.
        /// </param>
        /// <param name="cause">
        ///  The cause of the code contract violation.
        /// </param>
        /// <param name="inner">
        ///  The inner exception.
        /// </param>
        public CodeContractViolationException(
            string message
            , ViolationType cause
            , Exception inner
        )
            : base(message, inner)
        {
            m_cause = cause;
        }
        #endregion

        #region properties

        /// <summary>
        ///  Gets the cause of the code contract violation.
        /// </summary>
        public ViolationType Cause
        {
            get
            {
                return m_cause;
            }
        }
        #endregion
    }
}
