using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Helper methods for handling XML.
    /// </summary>
    public static class XmlUtilities
    {
        #region constants

        private static class Constants
        {
            internal const string FailureMessage_MissingChildXmlElement_2 =
                "Could not find child element '{0}' in XML '{1}'";
            internal const string FailureMessage_MissingXmlAttribute_2 =
                "Could not find attribute '{0}' in XML '{1}'";
        }
        #endregion

        #region methods

        /// <summary>
        ///  Gets the first (in document order) child element with the
        ///  specified name.
        /// </summary>
        /// <param name="element">The parent element. Must not be <c>null</c>.</param>
        /// <param name="name">
        ///  The name to match. At least one element matching this name must
        ///  exist as a child element. Must not be <c>null</c> or whitespace.
        /// </param>
        /// <returns>An XElement that matches the specified name.</returns>
        /// <exception cref="ApplicationException">
        ///  There was no child element with the provided name.
        /// </exception>
        public static
            XElement
            GetFirstChildElementByName(
                  XElement element
                , string name
            )
        {
            Requires.ThatArgumentIsNotNull(element, "element");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(name, "name");

            var childElement = element.Element(name);

            Requires.That(
                  null != childElement
                , String.Format(
                      Constants.FailureMessage_MissingChildXmlElement_2
                    , name
                    , element.ToString()
                )
            );

            return childElement;
        }

        /// <summary>
        ///  Gets one or more (in document order) child elements with the
        ///  specified name.
        /// </summary>
        /// <param name="element">The parent element. Must not be <c>null</c>.</param>
        /// <param name="name">
        ///  The name to match. At least one element matching this name must
        ///  exist as a child element. Must not be <c>null</c> or whitespace.
        /// </param>
        /// <returns>
        ///  An IEnumerable that contains one or more XElements that match the
        ///  specified name.
        /// </returns>
        /// <exception cref="ApplicationException">
        ///  There was no child element with the provided name.
        /// </exception>
        public static
            IEnumerable<XElement>
            GetOneOrMoreChildElementsByName(
                  XElement element
                , string name
            )
        {
            Requires.ThatArgumentIsNotNull(element, "element");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(name, "name");

            var childElements = element.Elements(name);

            Requires.That(
                  0 < childElements.Count()
                , String.Format(
                      Constants.FailureMessage_MissingChildXmlElement_2
                    , name
                    , element.ToString()
                )
            );

            return childElements;
        }

        /// <summary>
        ///  Gets the attribute with the specified name.
        /// </summary>
        /// <param name="element">
        ///  The element that contains the attribute. Must not be <c>null</c>.
        /// </param>
        /// <param name="name">
        ///  The name to match. At least one attribute matching this name must
        ///  exist. Must not be <c>null</c> or whitespace.
        /// </param>
        /// <returns>An XAttribute that matches the specified name.</returns>
        /// <exception cref="ApplicationException">
        ///  There was no attribute with the provided name.
        /// </exception>
        public static
            XAttribute
            GetAttributeByName(
                  XElement element
                , string name
            )
        {
            Requires.ThatArgumentIsNotNull(element, "element");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(name, "name");

            var attribute = element.Attribute(name);

            Requires.That(
                  null != attribute
                , String.Format(
                      Constants.FailureMessage_MissingXmlAttribute_2
                    , name
                    , element.ToString()
                )
            );

            return attribute;
        }
        #endregion
    }
}
