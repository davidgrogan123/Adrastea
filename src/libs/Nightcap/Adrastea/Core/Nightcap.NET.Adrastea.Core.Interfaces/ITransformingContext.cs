namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  The context to be used during transformations. This is a simple 
    ///  container for more useful interfaces to be retrieved.
    /// </summary>
    public interface ITransformingContext : IDisposable
    {
        #region methods 

        /// <summary>
        ///  Gets an interface that is supported by the transforming context.
        /// </summary>
        /// <typeparam name="T">
        ///  The type of interface to get.
        /// </typeparam>
        /// <param name="o">
        ///  When this method returns, contains the value associated with the
        ///  interface, if the interface is found; otherwise, the default
        ///  value. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        ///  <c>true</c> if the interface is supported; otherwise, 
        ///  <c>false</c>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        ///  Thrown if called after this context has been disposed.
        /// </exception>
        bool TryGetInterface<T>(out T o) where T : class;
        #endregion
    }
}
