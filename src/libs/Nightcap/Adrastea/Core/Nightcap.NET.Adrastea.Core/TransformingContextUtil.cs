using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Helper methods for creating instances of ITransformingContext.
    /// </summary>
    public static class TransformingContextUtil
    {
        #region methods

        /// <summary>
        ///  Creates a transforming context instance that exposes the file 
        ///  containing the content to be transformed.
        /// </summary>
        /// <param name="fileEntry">
        ///  An entry for the content to be transformed. May not be 
        ///  <c>null</c>.
        /// </param>
        /// <returns>
        ///  A transforming context that provides an entry for the file
        ///  to be transformed.
        /// </returns>
        public static ITransformingContext CreateFileEntryTransformingContext(
            string fileEntry
        )
        {
            Requires.ThatArgumentIsNotNull(fileEntry, "fileEntry");

            object[] contexts = { new FileEntryTransformingContext(fileEntry) };

            return new TransformingContext(contexts);
        }

        /// <summary>
        ///  Creates a transforming context instance that exposes the content
        ///  to be transformed through an XmlReader.
        /// </summary>
        /// <param name="reader">
        ///  An XmlReader for the content to be transformed. May not be 
        ///  <c>null</c>.
        /// </param>
        /// <returns>
        ///  A transforming context that provides an XmlReader to access the
        ///  content to be transformed.
        /// </returns>
        /// <exception cref="XmlException">
        ///  Thrown when the <paramref name="reader"/> does not contain valid
        ///  <c>XML</c>.
        /// </exception>
        /// <remarks>
        ///  The reader provided to this method will be read during execution. 
        ///  The XmlReaders created by the returned ITransformingContext do 
        ///  not share a stream and so this context may be shared.
        /// </remarks>
        public static ITransformingContext CreateXmlReaderTransformingContext(
            XmlReader reader
        )
        {
            Requires.ThatArgumentIsNotNull(reader, "reader");

            object[] contexts = { new XmlReaderTransformingContext(reader) };

            return new TransformingContext(contexts);
        }

        /// <summary>
        ///  Creates a transforming context instance that exposes the content
        ///  to be transformed through an XmlReader.
        /// </summary>
        /// <param name="document">
        ///  An XDocument for the content to be transformed. May not be 
        ///  <c>null</c>.
        /// </param>
        /// <returns>
        ///  A transforming context that provides an XmlReader to access the
        ///  content to be transformed.
        /// </returns>
        /// <remarks>
        ///  The XmlReaders created by the returned ITransformingContext do 
        ///  not share a stream and so this context may be shared.
        /// </remarks>
        public static ITransformingContext CreateXmlReaderTransformingContext(
            XDocument document
        )
        {
            Requires.ThatArgumentIsNotNull(document, "document");

            object[] contexts = { new XmlReaderTransformingContext(document) };

            return new TransformingContext(contexts);
        }

        /// <summary>
        ///  Creates a transforming context instance that exposes the content
        ///  to be transformed through a StreamReader.
        /// </summary>
        /// <param name="stream">
        ///  A stream for the content to be transformed. May not be 
        ///  <c>null</c>. This stream will be closed when this context is
        ///  Disposed.
        /// </param>
        /// <returns>
        ///  A transforming context that allows the content to be transformed
        ///  to be accessed through an StreamReader.
        /// </returns>
        /// <remarks>
        ///  Closing of the stream exposed by the returned ITransformingContext
        ///  will not close the underlying stream, and so subsequent readers
        ///  may be created. However, streams should not be shared.
        ///  readers. 
        /// </remarks>
        public static ITransformingContext CreateStreamReaderTransformingContext(
            Stream stream
        )
        {
            Requires.ThatArgumentIsNotNull(stream, "stream");

            object[] contexts = { new StreamReaderTransformingContext(stream) };

            return new TransformingContext(contexts);
        }
        #endregion
    }
}
