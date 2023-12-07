namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  Defines a series of transformation steps to be applied to an input.
    /// </summary>
    public interface ITransformer
    {
        #region methods

        /// <summary>
        ///  Performs the transformation.
        /// </summary>
        /// <exception>TBD</exception>
        /// <remarks>
        ///  <para>
        ///   <b>Events</b>
        ///  </para>
        ///  <para>
        ///   <b>EventHandler{FileStartTransformEventArgs}</b>: 
        ///   Generated before any transforms if a handler was provided at 
        ///   construction.
        ///  </para>
        ///  <para>
        ///   <b>EventHandler{FileEndTransformEventArgs}</b>: 
        ///   Generated after all transformations have been completed if a 
        ///   handler was provided at construction.
        ///  </para>
        /// </remarks>
        void Transform();
        #endregion
    }
}
