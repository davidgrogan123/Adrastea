namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  A transforming context to access the content to be transformed
    ///  through a TextStream. This should be exposed from 
    ///  <c>ITransformingContext</c>.
    /// </summary>
    public interface IStreamReaderTransformingContext
    {
        /// <summary>
        ///  Creates a StreamReader to access the content to be transformed.
        ///  Only one reader should be created at a time. Closing this reader
        ///  will not close the underlying stream.
        /// </summary>
        /// <returns>
        ///  A StreamReader for the content to be transformed.
        /// </returns>
        StreamReader CreateStreamReader();
    }
}
