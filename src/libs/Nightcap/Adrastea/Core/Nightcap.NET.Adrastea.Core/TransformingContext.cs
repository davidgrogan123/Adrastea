using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.Adrastea.Core
{
    /// <inheritdoc/>
    public sealed class TransformingContext : ITransformingContext
    {
        #region constants

        private static class Constants
        {
            internal static class FailureMessages
            {
                internal const string ContextDisposed =
                    "TransformingContext has been disposed";
            }
        }
        #endregion

        #region fields

        private readonly object[] m_interfaces;
        private bool m_disposed;
        #endregion

        #region construction

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="interfaces">
        ///  The interfaces that are supported by this transforming context. 
        ///  May not be <c>null</c>.
        /// </param>
        public TransformingContext(
            object[] interfaces
        )
        {
            Requires.ThatArgumentIsNotNull(interfaces, "interfaces");

            m_interfaces = interfaces;
            m_disposed = false;
        }
        #endregion

        #region implementation

        /// <inheritdoc/>
        public bool TryGetInterface<T>(out T o) where T : class
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException(
                    Constants.FailureMessages.ContextDisposed
                );
            }

            o = m_interfaces.FirstOrDefault(x => x is T) as T;

            return o != default(T);
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
                foreach (var o in m_interfaces)
                {
                    IDisposable d = o as IDisposable;

                    if (null != d)
                    {
                        d.Dispose();
                    }
                }
            }

            m_disposed = true;
        }
        #endregion
    }
}
