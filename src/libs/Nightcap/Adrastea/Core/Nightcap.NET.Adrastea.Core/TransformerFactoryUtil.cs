using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    #region aliases

    // Associates a matcher with a collection of transformations
    using MatcherTransformations = System.Tuple<
          IMatcher
        , System.Collections.Generic.IReadOnlyCollection<
              ITransformation
          >
    >;
    #endregion

    /// <summary>
    ///  A static utility class that provides support methods to transformer 
    ///  factories.
    /// </summary>
    internal static class TransformerFactoryUtil
    {
        #region constants

        private static class Constants
        {
            internal const string TransformersElementName = "transformers";
            internal const string TransformerElementName = "transformer";
            internal const string TransformationsElementName = "transformations";
            internal const string TransformationElementName = "transformation";
            internal const string MatcherElementName = "matcher";
        }
        #endregion

        #region support methods

        /// <summary>
        ///  A support method for the creation of matchers and transformations
        ///  to be used by a transformer factory.
        /// </summary>
        /// <param name="config">
        ///  The <c>XML</c> configuration defining how to create transformers.
        /// </param>
        /// <param name="matcherFactory">
        ///  Factory that will be used to construct file entry matcher objects.
        /// </param>
        /// <param name="transformationFactory">
        ///  Factory that will be used to construct transformation objects.
        /// </param>
        internal static
            IReadOnlyCollection<MatcherTransformations>
            CreateMatchersAndTransformations(
                  XmlReader config
                , IMatcherFactory matcherFactory
                , ITransformationFactory transformationFactory
            )
        {
            Requires.ThatArgumentIsNotNull(config, "config");
            Requires.ThatArgumentIsNotNull(matcherFactory, "matcherFactory");
            Requires.ThatArgumentIsNotNull(transformationFactory, "transformationFactory");

            // find the xml elements
            var elms =
                XmlUtilities.GetOneOrMoreChildElementsByName(
                      XElement.Load(
                            config
                      )
                    , Constants.TransformerElementName
                );

            // create the objects
            var objs =
                from elm in elms
                select new MatcherTransformations(
                      CreateMatcher(
                            elm
                          , matcherFactory
                      )
                    , CreateTransformations(
                          elm
                        , transformationFactory
                    )
                );

            return objs.ToList().AsReadOnly();
        }
        #endregion

        #region private methods

        private static
            IReadOnlyCollection<ITransformation>
            CreateTransformations(
                  XElement parent
                , ITransformationFactory factory
            )
        {
            // find the xml elements
            var elms =
                XmlUtilities.GetOneOrMoreChildElementsByName(
                      XmlUtilities.GetFirstChildElementByName(
                            parent
                          , Constants.TransformationsElementName
                      )
                    , Constants.TransformationElementName
                );

            // create the objects
            var objs =
                from elm in elms
                select factory.CreateTransformation(
                      elm.CreateReader()
                );

            return objs.ToList().AsReadOnly();
        }

        private static
            IMatcher
            CreateMatcher(
                  XElement parent
                , IMatcherFactory factory
            )
        {
            // find the xml element
            var elm = XmlUtilities.GetFirstChildElementByName(
                  parent
                , Constants.MatcherElementName
            );

            // create the object
            var obj = factory.CreateMatcher(
                  elm.CreateReader()
            );

            return obj;
        }
        #endregion
    }
}
