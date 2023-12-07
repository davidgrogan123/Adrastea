using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using NLog;
using System.Text;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  A Transforming Context where the content to be transformed may be
    ///  accessed through a StreamReader.
    /// </summary>
    public sealed class StreamReaderTransformingContext
        : IStreamReaderTransformingContext, IDisposable
    {
        #region constants

        private static class Constants
        {
            /// <summary>
            ///  These constants are the values used in the default 
            ///  StreamReader constructor. To use the LeaveOpen parameter
            ///  these must also be supplied.
            /// </summary>
            internal static class StreamReaderConstructorArguments
            {
                internal const bool DetectEncoding = false;
                internal const int BufferSize = 1024;
                internal const bool LeaveOpen = true;
            }
        }
        #endregion

        #region fields

        private readonly Stream m_stream;
        private bool m_disposed;

        private static readonly Encoding sm_Encoding;
        private static Logger sm_logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region construction

        /// <summary>
        ///  Static constructor.
        /// </summary>
        static StreamReaderTransformingContext()
        {
            try
            {
                sm_Encoding = Encoding.UTF8;
            }
            catch (Exception x)
            {
                sm_logger.Error(
                    x 
                  , "StreamReaderTransformingContext static initialization constructor failed"
                );

                throw;
            }
        }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="stream">
        ///  The stream that contains the content to be transformed. May not
        ///  be <c>null</c>.
        /// </param>
        /// <remarks>
        ///  Disposing of this context will dispose the underlying stream.
        /// </remarks>
        public StreamReaderTransformingContext(Stream stream)
        {
            Requires.ThatArgumentIsNotNull(stream, "stream");

            m_stream = stream;
            m_disposed = false;
        }
        #endregion

        #region implementation

        /// <inheritdoc />
        public StreamReader CreateStreamReader()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("StreamReaderTransformingContext has been disposed.");
            }

            return new StreamReader(
                  m_stream
                , sm_Encoding
                , Constants.StreamReaderConstructorArguments.DetectEncoding
                , Constants.StreamReaderConstructorArguments.BufferSize
                , Constants.StreamReaderConstructorArguments.LeaveOpen
            );
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        void Dispose(bool disposing)
        {
            if (m_disposed)
            {
                return;
            }

            if (disposing)
            {
                m_stream.Dispose();
            }

            m_disposed = true;
        }
        #endregion
    }
}
