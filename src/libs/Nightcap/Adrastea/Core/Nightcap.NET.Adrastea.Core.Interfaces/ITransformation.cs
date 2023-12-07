namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  A single transformation step that can be applied as part of ETL. 
    ///  There may be multiple transformation steps implemented by a single 
    ///  transformer.
    /// </summary>
    public interface ITransformation
    {
        #region methods

        /// <summary>
        ///  Transforms data contained by the text reader, and writes it to a
        ///  text writer.
        /// </summary>
        /// <param name="context">
        ///  Contains the data to transform.
        /// </param>
        /// <param name="eventHandlers">
        ///  A collection of event handlers that are supported. May not be 
        ///  <c>null</c>.
        /// </param>
        /// <returns>
        ///  A context containing the transformed data.
        /// </returns>
        /// <exception>TBD</exception>
        /// <remarks>
        ///  <para>
        ///   <b>Events</b>
        ///  </para>
        /// </remarks>
        ITransformingContext Transform(
              ITransformingContext context
            , IEnumerable<Delegate> eventHandlers
        );
        #endregion
    }
}
