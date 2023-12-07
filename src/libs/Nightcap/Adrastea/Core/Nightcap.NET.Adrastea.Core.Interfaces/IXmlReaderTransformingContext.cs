using System.Xml;

namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  A transforming context to access the content to be transformed
    ///  through an XmlReader. This should be exposed from 
    ///  <c>ITransformingContext</c>.
    /// </summary>
    public interface IXmlReaderTransformingContext
    {
        /// <summary>
        ///  Creates an XmlReader to access the content to be transformed.
        /// </summary>
        /// <returns>
        ///  An XmlReader.
        /// </returns>
        XmlReader CreateXmlReader();
    }
}
