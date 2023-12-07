using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  A Transforming Context where the content to be transformed may be
    ///  accessed through a FileEntry.
    /// </summary>
    public sealed class FileEntryTransformingContext
        : IFileEntryTransformingContext
    {
        #region fields

        private readonly string m_fileEntry;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="fileEntry">
        ///  A FileEntry for the file to be transformed. May not be 
        ///  <c>null</c> .
        /// </param>
        public FileEntryTransformingContext(string fileEntry)
        {
            Requires.ThatArgumentIsNotNull(fileEntry, "fileEntry");

            m_fileEntry = fileEntry;
        }
        #endregion

        #region accessors

        /// <inheritdoc />
        public string FileEntry
        {
            get
            {
                return m_fileEntry;
            }
        }
        #endregion
    }
}
