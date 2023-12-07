namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Event data provided after transformations have completed. If an event
    ///  that supports <see cref="FileTransformStartEventArgs"/> is generated
    ///  it can be expected that another event supporting this event data will
    ///  also be generated.
    /// </summary>
    public sealed class FileTransformEndEventArgs : EventArgs
    {
        #region fields

        private readonly bool m_successfullyProcessed;
        #endregion

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="successfullyProcessed">
        ///  Indicates if the file was transformed successfully.
        /// </param>
        public FileTransformEndEventArgs(
            bool successfullyProcessed
        )
        {
            m_successfullyProcessed = successfullyProcessed;
        }

        /// <summary>
        ///  <c>true</c> if the file was transformed successfully, otherwise,
        ///  <c>false</c>.
        /// </summary>
        public bool SuccessfullyProcessed
        {
            get
            {
                return m_successfullyProcessed;
            }
        }
    }
}
