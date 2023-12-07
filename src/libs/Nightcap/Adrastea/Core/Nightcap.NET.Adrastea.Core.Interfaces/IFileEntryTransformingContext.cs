namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  A transforming context to access the content to be transformed 
    ///  through a file entry. This should be exposed from 
    ///  <c>ITransformingContext</c>.
    /// </summary>
    public interface IFileEntryTransformingContext
    {
        /// <summary>
        ///  An entry for the file to be transformed.
        /// </summary>
        string FileEntry
        {
            get;
        }
    }
}
