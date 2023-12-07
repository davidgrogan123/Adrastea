namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    /// Creates instances of transformers.
    /// </summary>
    public interface IFileEntryTransformerFactory
    {
        #region methods

        /// <summary>
        /// Creates all transformers that are required to be performed on the
        /// specified file as a read only collection.
        /// </summary>
        /// <param name="fileEntry">
        /// The input file. May not be <c>null</c>.
        /// </param>
        /// <returns>
        /// A read only collection of transformers that should be applied to
        /// the file.
        /// </returns>
        /// <exception>TBC</exception>
        IReadOnlyCollection<ITransformer> CreateTransformers(
              string fileEntry
        );
        #endregion
    }
}
