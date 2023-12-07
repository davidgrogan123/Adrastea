using Nightcap.Adrastea.Core.Events;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.NET.Core.IO;
using NLog;
using System.Xml;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Performs one or more transformations given a file entry.
    /// </summary>
    /// <remarks>
    ///  If the transformation is successful then the transformed data will 
    ///  be written to the output directory, and the input file will be moved 
    ///  to the done directory. If the transformation is unsuccessful then 
    ///  the input file will be moved to the failed directory, and no further
    ///  processing will take place. If the input file has been locked then 
    ///  no processing will take place.
    /// </remarks>
    public sealed class FileEntryTransformer : ITransformer
    {
        #region constants

        private static class Constants
        {
            internal static class FailureMessages
            {
                internal const string NoSupportedInterfaces =
                    "Transforming Context does not contain supported interface";
                internal const string FilePathContainsInvalidCharacters_1 =
                    "The file path '{0}' contains invalid characters";
                internal const string FilePathTooLong_1 =
                    "The file path '{0}' is too long";
            }

            internal static class WriteNodeParameters
            {
                internal const bool CopyDefaultAttributes = false;
            }
        }
        #endregion

        #region fields

        private readonly IReadOnlyCollection<ITransformation> m_transformations;
        private readonly XmlWriterSettings m_writerSettings;
        private readonly string m_fileEntry;
        private readonly string m_outputDirectory;
        private readonly string m_outputFilePrefix;
        private readonly IEnumerable<Delegate> m_eventHandlers;
        private readonly IEnumerable<Delegate> m_intermediateEventHandlers;
        private static Logger sm_logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region construction

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="transformations">
        ///  The transformations that will be applied by this transformer.
        ///  May not be <c>null</c> or Empty.
        /// </param>
        /// <param name="fileEntry">
        ///  The file entry that will be transformed. Must not be <c>null</c>.
        /// </param>
        /// <param name="outputDirectory">
        ///  The directory that will contain transform output for successfully
        ///  transformed files. May not be <c>null</c>.
        /// </param>
        /// <param name="outputFilePrefix">
        ///  The prefix that will be added to the output file. May not be <c>null</c>.
        /// </param>
        /// <exception>TBD</exception>
        /// <param name="eventHandlers">
        ///  A collection of event handlers that are supported. May not be 
        ///  <c>null</c>.
        /// </param>
        public FileEntryTransformer(
              IReadOnlyCollection<ITransformation> transformations
            , string fileEntry
            , string outputDirectory
            , string outputFilePrefix
            , IEnumerable<Delegate> eventHandlers
        )
        {
            Requires.ThatArgumentIsNotNull(transformations, "transformations");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(fileEntry, "fileEntry");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(outputDirectory, "outputDirectory");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(outputFilePrefix, "outputFilePrefix");
            Requires.ThatArgument(() => transformations.Any(), "transformations", "Transformations may not be empty");
            Requires.ThatArgumentIsNotNull(eventHandlers, "eventHandlers");

            m_transformations = transformations;
            m_outputDirectory = outputDirectory;
            m_outputFilePrefix = outputFilePrefix;
            m_eventHandlers = eventHandlers;

            m_writerSettings = new XmlWriterSettings
            {
                CloseOutput = true
                ,
                Indent = true
            };

            // TODO: Determine path validity

            m_fileEntry = fileEntry;
        }
        #endregion

        #region methods

        /// <inheritdoc/>
        public void Transform()
        {
            // write a transform start message to the log
            sm_logger.Info("Starting transform for file '{0}", m_fileEntry);

            EventUtil.GenerateEvent<FileTransformStartEventArgs>(
                  m_eventHandlers
                , this
                , new FileTransformStartEventArgs(m_fileEntry)
            );

            bool transformSuccess = false;

            try
            {

                using (Stream fileStream = FileUtilities.CreateReadableStream(m_fileEntry))
                using (ITransformingContext contextIn =
                        TransformingContextUtil
                            .CreateStreamReaderTransformingContext(fileStream)
                )
                {
                    using (ITransformingContext contextOut =
                        TransformationUtil.ExecuteTransformations(
                              m_transformations
                            , contextIn
                            , m_intermediateEventHandlers
                        )
                    )
                    {
                        var outputFilePath = Path.Combine(
                            m_outputDirectory
                            , String.Concat(
                                  m_outputFilePrefix
                                , Path.GetFileName(m_fileEntry)
                            )
                        );

                        transformSuccess = true;

                        WriteToFile(contextOut, outputFilePath);
                    }
                }
            }
            finally
            {
                EventUtil.GenerateEvent<FileTransformEndEventArgs>(
                      m_eventHandlers
                    , this
                    , new FileTransformEndEventArgs(transformSuccess)
                );
            }

            // write a transform complete message to the log
            sm_logger.Info("Successfully completed transform for file '{0}", m_fileEntry);
        }
        #endregion

        #region private support methods

        private void WriteToFile(
              ITransformingContext context
            , string outputFilePath
        )
        {
            IXmlReaderTransformingContext xmlContext;
            IStreamReaderTransformingContext streamContext;

            if (context.TryGetInterface(out xmlContext))
            {
                using (XmlReader reader = xmlContext.CreateXmlReader())
                using (XmlWriter writer = XmlWriter.Create(
                          FileUtilities.CreateWritableStream(outputFilePath)
                        , m_writerSettings
                    )
                )
                {
                    writer.WriteNode(
                          reader
                        , Constants.WriteNodeParameters.CopyDefaultAttributes
                    );
                }
            }
            else if (context.TryGetInterface(out streamContext))
            {
                using (StreamReader reader = streamContext.CreateStreamReader())
                {
                    FileUtilities.WriteAllText(outputFilePath, reader.ReadToEnd());
                }
            }
            else
            {
                // TODO: Determine correct exception type.
                throw new ApplicationException(
                    Constants.FailureMessages.NoSupportedInterfaces
                );
            }
        }
        #endregion
    }
}
