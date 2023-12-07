namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  Performs a match against a context.
    /// </summary>
    public interface IMatcher
    {
        #region methods

        /// <summary>
        ///  True if a match is found.
        /// </summary>
        /// <param name="context">
        ///  The context that will be used to determine whether a match is
        ///  found. May not be <c>null</c>.
        /// </param>
        /// <returns>
        ///  <c>true</c> if the a match is found; otherwise, <c>false</c>.
        /// </returns>
        /// <exception>TBD</exception>
        bool IsMatch(
            IMatchingContext context
        );
        #endregion
    }
}
