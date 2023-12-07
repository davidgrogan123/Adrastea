using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  An implementation of IMatchingContext that supports a fixed array of
    ///  interfaces that are passed to its constructor.
    /// </summary>
    public sealed class MatchingContext : IMatchingContext
    {
        #region fields

        private readonly object[] m_interfaces;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="interfaces">
        ///  The interfaces that are supported by this matching context. Must
        ///  not be <c>null</c>.
        /// </param>
        public
            MatchingContext(
                  object[] interfaces
            )
        {
            Requires.ThatArgumentIsNotNull(interfaces, "interfaces");

            m_interfaces = interfaces;
        }

        #endregion

        #region methods

        /// <inheritdoc/>
        public bool
            TryGetInterface<T>(
                  out T o
            )
            where T : class
        {
            o = m_interfaces.FirstOrDefault(
                  x => x is T
            ) as T;

            return o != default(T);
        }
        #endregion
    }

    /// <inheritdoc/>
    public sealed class FileEntryMatchingContext : IFileEntryMatchingContext
    {
        #region fields

        private readonly string m_fileEntry;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="fileEntry">
        ///  The file entry. Must not be <c>null</c> or whitespace.
        /// </param>
        public
            FileEntryMatchingContext(
                  string fileEntry
            )
        {
            Requires.ThatArgumentIsNotNull(fileEntry, "fileEntry");

            m_fileEntry = fileEntry;
        }
        #endregion

        #region methods

        /// <inheritdoc/>
        public
            string
            FileEntry
        {
            get
            {
                return m_fileEntry;
            }
        }
        #endregion
    }
}
