using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Event data provided before any transformations begin. If a method
    ///  generates an event supporting this event data it must also grantee
    ///  an event with <see cref="FileTransformEndEventArgs"/> is also 
    ///  generated.
    /// </summary>
    public sealed class FileTransformStartEventArgs : EventArgs
    {
        #region fields

        private readonly string m_filePath;
        #endregion

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="filePath">
        ///  The path to the file that is being transformed.
        /// </param>
        public FileTransformStartEventArgs(
            string filePath
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            m_filePath = filePath;
        }

        /// <summary>
        ///  The path to the file that is being transformed.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_filePath;
            }
        }
    }
}
