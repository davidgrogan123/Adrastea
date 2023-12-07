using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  A Transforming Context where the content to be transformed may be
    ///  accessed through an XmlReader.
    /// </summary>
    /// <remarks>
    ///  TODO: This implementation should be changed so it does not rely on
    ///  an XDocument. A possible solution is a custom implementation of
    ///  XmlReader, where a reader can be created from an existing XmlReader
    ///  that cannot close the original reader.
    /// </remarks>
    public sealed class XmlReaderTransformingContext
        : IXmlReaderTransformingContext
    {
        #region fields

        private readonly XDocument m_doc;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="reader">
        ///  The XmlReader containing the content to be transformed. May not
        ///  be <c>null</c> .
        /// </param>
        /// <exception cref="XmlException">
        ///  Thrown when the <paramref name="reader"/> does not contain valid
        ///  <c>XML</c>.
        /// </exception>
        /// <remarks>
        ///  The <paramref name="reader"/> will be read.
        /// </remarks>
        public XmlReaderTransformingContext(XmlReader reader)
        {
            Requires.ThatArgumentIsNotNull(reader, "reader");

            m_doc = XDocument.Load(reader);
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="document">
        ///  The XDocument containing the content to be transformed. May not
        ///  be <c>null</c> .
        /// </param>
        public XmlReaderTransformingContext(XDocument document)
        {
            Requires.ThatArgumentIsNotNull(document, "document");

            m_doc = document;
        }
        #endregion

        #region implementation

        /// <inheritdoc />
        public XmlReader CreateXmlReader()
        {
            return m_doc.CreateReader();
        }
        #endregion
    }
}
