namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  The context to use when performing a match.
    /// </summary>
    /// <remarks>
    ///  This is designed to be a simple container from which more useful
    ///  interfaces for performing a match should be retrieved. It exposes a
    ///  single <c>TryGetInterface</c> method that allows determination of
    ///  whether a particular known interface might be supported, and retrieval
    ///  if so.
    /// 
    ///  In recent versions of C# (7+) the out parameter doesn't need to be
    ///  declared before using this method, which allows the following
    ///  convenient idiom to be used:
    /// 
    ///  if (context.TryGetInterface(out IFileEntryMatchingContext o))
    ///  {
    ///      // use o param here
    ///  }
    /// </remarks>
    public interface IMatchingContext
    {
        #region methods

        /// <summary>
        ///  Gets an interface that is supported by the matching context.
        /// </summary>
        /// <typeparam name="T">The type of interface to get.</typeparam>
        /// <param name="o">
        ///  When this method returns, contains the value associated with the
        ///  interface, if the interface is found; otherwise, the default
        ///  value. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///  <c>true</c> if the interface is supported; otherwise, <c>false</c>.
        /// </returns>
        bool TryGetInterface<T>(
              out T o
        ) where T : class;
        #endregion
    }

    /// <summary>
    ///  A context that is used to match against a file entry. This should be
    ///  exposed from <c>IMatchingContext</c>.
    /// </summary>
    public interface IFileEntryMatchingContext
    {
        #region methods

        /// <summary>
        ///  The file entry.
        /// </summary>
        string FileEntry
        {
            get;
        }
        #endregion
    }
}
