using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Creates instances of transformations.
    /// </summary>
    /// <remarks>
    ///  TODO: Consider caching transformations.
    /// </remarks>
    public sealed class TransformationFactory : ITransformationFactory
    {
        #region constants

        private static class Constants
        {
            internal const string TypeAttributeName = "type";

            public static class TypeName
            {
                internal const string CsvToXml = "CsvToXmlTransformation";
            }

            public static class XmlSplitterAttributes
            {
                internal const string CollectionRoot = "collectionRoot";
            }

            public static class FailureMessages
            {
                internal const string TypeNotFound_3 =
                    "Could not create a transformation type of '{0}' from attribute '{1}' in "
                    + "element '{2}'. This type doesn't match any of the pre-configured "
                    + "types";
                internal const string FailedToReadFile_1 =
                    "Could not read text from file '{0}'";
            }
        }
        #endregion

        #region fields

        private readonly IMatchingContextFactory m_matchingContextFactory;
        private readonly IMatcherFactory m_matcherFactory;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor overload that allows setting code execution
        ///  characteristics.
        /// </summary>
        /// <param name="matchingContextFactory">
        ///  The MatchingContextFactory to create Matching Contexts.
        ///  May not be <c>null</c>.
        /// </param>
        /// <param name="matcherFactory">
        ///  The MatcherFactory to be used to create Matchers. May not be <c>null</c>.
        /// </param>
        /// <exception>TBD</exception>
        public
            TransformationFactory(
                  IMatchingContextFactory matchingContextFactory
                , IMatcherFactory matcherFactory
            )
        {
            Requires.ThatArgumentIsNotNull(matchingContextFactory, "matchingContextFactory");
            Requires.ThatArgumentIsNotNull(matcherFactory, "matcherFactory");

            m_matchingContextFactory = matchingContextFactory;
            m_matcherFactory = matcherFactory;
        }
        #endregion

        #region methods

        private
            ITransformation
            CreateCsvToXmlTransformation(
                  XElement elm
            )
        {
            return new CsvToXmlTransformation();
        }

        /// <inheritdoc/>
        public
            ITransformation
            CreateTransformation(
                XmlReader config
            )
        {
            Requires.ThatArgumentIsNotNull(config, "config");

            var elm = XElement.Load(
                  config
            );

            // retrieve the type of transformer to create
            var typeName = XmlUtilities.GetAttributeByName(
                  elm
                , Constants.TypeAttributeName
            ).Value;

            // create the new transformation
            switch (typeName)
            {
                case Constants.TypeName.CsvToXml:
                    return CreateCsvToXmlTransformation(
                        elm
                    );

                default:
                    // TODO: determine correct exception type
                    throw new ApplicationException(
                          String.Format(
                                Constants.FailureMessages.TypeNotFound_3
                              , typeName
                              , Constants.TypeAttributeName
                              , elm
                          )
                    );
            }
        }
        #endregion
    }
}
