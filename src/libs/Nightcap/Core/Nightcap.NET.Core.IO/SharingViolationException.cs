using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.NET.Core.IO
{
    /// <summary>
    ///  Thrown when a process cannot access a file because it is being used by
    ///  another process.
    /// </summary>
    public sealed class SharingViolationException : Exception
    {
        #region construction

        /// <summary>
        ///  Initializes a new instance of the Exception class.
        /// </summary>
        public SharingViolationException()
        {
        }

        /// <summary>
        ///  Initializes a new instance of the Exception class with a
        ///  specified error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        public SharingViolationException(
              string message
        )
            : base(
                  message
            )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(message, "message");
        }

        /// <summary>
        ///  Initializes a new instance of the Exception class with a
        ///  specified error message and a reference to the inner exception
        ///  that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="inner">The inner exception.</param>
        public SharingViolationException(
              string message
            , Exception inner
        )
            : base(
                  message
                , inner
            )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(message, "message");
            Requires.ThatArgumentIsNotNull(inner, "inner");
        }
        #endregion
    }
}
