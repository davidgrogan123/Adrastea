using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;

namespace Nightcap.Adrastea.Core
{
    #region aliases

    // Associates a matcher with a collection of transformations
    using MatcherTransformations = System.Tuple<
          Interfaces.IMatcher
        , System.Collections.Generic.IReadOnlyCollection<
              Interfaces.ITransformation
          >
    >;
    #endregion

    /// <summary>
    /// Creates instances of File Entry Transformers.
    /// </summary>
    public sealed class FileEntryTransformerFactory
        : IFileEntryTransformerFactory
    {
        #region fields

        private readonly IReadOnlyCollection<MatcherTransformations>
            m_matcherTransformationsCollection;

        private readonly IMatchingContextFactory m_matchingContextFactory;
        private readonly IEnumerable<Delegate> m_eventHandlers;

        private readonly string m_outputDirectory;
        private readonly string m_outputFilePrefix;
        #endregion

        #region constructors

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <remarks>
        ///  The transformersXmlConfig parameter to the constructor must contain
        ///  valid XML configuration for creating transformers. It is assumed
        ///  that this has already been validated via XSD or some other
        ///  technique.
        ///  
        ///  It must contain at least one transformer child element. Each
        ///  transformer element must contain a single matcher element, and a
        ///  single transformations collection as child elements. The
        ///  transformations child collection must contain at least one
        ///  transformation element.
        ///  
        ///  Example configuration might look like this:
        ///
        ///  &lt;transformers>
        ///    &lt;transformer>
        ///      &lt;matcher type=""regex"">input.csv&lt;/matcher>
        ///      &lt;transformations>
        ///        &lt;transformation type=""xsl"">some_file.xsl&lt;/transformation>
        ///      &lt;/transformations>
        ///    &lt;/transformer>
        ///  &lt;/transformers>
        /// 
        /// </remarks>
        /// <param name="transformersXmlConfig">
        ///  The XML configuration defining how to create transformers.
        /// </param>
        /// <param name="matcherFactory">
        ///  Factory that will be used to construct file entry matcher objects.
        /// </param>
        /// <param name="transformationFactory">
        ///  Factory that will be used to construct transformation objects.
        /// </param>
        /// <param name="matchingContextFactory">
        ///  Factory that will be used to construct matching context objects.
        /// </param>
        /// <param name="eventHandlers">
        ///  A collection of event handlers that are supported. May not be 
        ///  <c>null</c>.
        /// </param>
        /// <param name="outputDirectory">
        ///  The directory that will contain transform output for successfully
        ///  transformed files. Must not be <c>null</c>.
        /// </param>
        /// <param name="outputFilePrefix">
        ///  The prefix that will be prepended to the output file. If there are
        ///  multiple output files then a numerical prefix will be appended to
        ///  the higher order file prefixes, so that each file name is
        ///  unique. For example: If the prefix is 'abc_', then 'abc_' will be
        ///  prepended to the first output file name, 'abc_1' will be prepended
        ///  to the second output file name, 'abc_2' to the third file name etc.
        ///  May not be <c>null</c>.
        /// </param>
        public FileEntryTransformerFactory(
              XmlReader transformersXmlConfig
            , IMatcherFactory matcherFactory
            , ITransformationFactory transformationFactory
            , IMatchingContextFactory matchingContextFactory
            , IEnumerable<Delegate> eventHandlers
            , string outputDirectory
            , string outputFilePrefix
        )
        {
            Requires.ThatArgumentIsNotNull(transformersXmlConfig, "transformersXmlConfig");
            Requires.ThatArgumentIsNotNull(matcherFactory, "matcherFactory");
            Requires.ThatArgumentIsNotNull(transformationFactory, "transformationFactory");
            Requires.ThatArgumentIsNotNull(matchingContextFactory, "matchingContextFactory");
            Requires.ThatArgumentIsNotNull(eventHandlers, "eventHandlers");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(outputDirectory, "outputDirectory");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(outputFilePrefix, "outputFilePrefix");

            m_matchingContextFactory = matchingContextFactory;
            m_eventHandlers = eventHandlers;
            m_outputDirectory = outputDirectory;
            m_outputFilePrefix = outputFilePrefix;

            m_matcherTransformationsCollection =
                TransformerFactoryUtil.CreateMatchersAndTransformations(
                      transformersXmlConfig
                    , matcherFactory
                    , transformationFactory
                );
        }
        #endregion

        #region methods

        private
            string
            CalculatePrefixForFileIndex(
                  int index
            )
        {
            if (index == 0)
            {
                return m_outputFilePrefix;
            }
            else
            {
                return m_outputFilePrefix + index;
            }
        }

        /// <inheritdoc/>
        public
            IReadOnlyCollection<ITransformer>
            CreateTransformers(
                  string fileEntry
            )
        {
            // create the matching context object that will be used to
            // determine which transformations need to be applied to the file
            var context = m_matchingContextFactory.CreateMatchingContext(
                  fileEntry
                , ContentType.FilePath
            );

            // the output directory for a successful transformation
            var successfulTransformationDir = m_outputDirectory;

            // work out which transformations are applicable for this file based
            // on whether their matcher 'matches'
            var transformations =
                from m in m_matcherTransformationsCollection
                where m.Item1.IsMatch(
                    context
                )
                select m.Item2;

            // create a collection of applicable transformers
            var transformers = transformations.Select(
                (t, i) => new FileEntryTransformer(
                      t
                    , fileEntry
                    , successfulTransformationDir
                    , CalculatePrefixForFileIndex(i)
                    , m_eventHandlers
                )
            );

            return transformers.ToList().AsReadOnly();
        }
        #endregion
    }
}
