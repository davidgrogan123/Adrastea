namespace Nightcap.Adrastea.Core.Interfaces
{
    #region types

    /// <summary>
    ///  Specifies the format of the content that a matcher can use to 
    ///  determine whether a match has been made.
    /// </summary>
    /// <remarks>
    ///  TODO: This hard coded approach isn't flexible. It needs to be 
    ///  updated to use a similar approach to ITransformingContext.
    ///  
    ///  See ITransformingContext for more information.
    /// </remarks>
    public enum ContentType
    {
        /// <summary>
        ///  Default value.
        /// </summary>
        None = 1,
        /// <summary>
        ///  Matchers can match against the file path.
        /// </summary>
        FilePath,
        /// <summary>
        ///  Matchers can match against XML content.
        /// </summary>
        XmlReader,
    }
    #endregion

    /// <summary>
    ///  Creates instances of matching contexts.
    /// </summary>
    public interface IMatchingContextFactory
    {
        #region methods

        /// <summary>
        ///  Creates a context used for matching data.
        /// </summary>
        /// <param name="content">
        ///  The content the matching context is to be created for. May not 
        ///  be <c>null</c>.
        /// </param>
        /// <param name="contentType">
        ///  Specifies the format of the content. May not be <c>null</c>.
        /// </param>
        /// <returns>
        /// A matching context that can be passed to the <c>IsMatch</c> method
        /// for objects that implement the <c>IMatcher</c> interface.
        /// </returns>
        /// <remarks>
        ///  TODO: This hard coded contentType approach isn't flexible. It 
        ///  needs to be updated to use a similar approach to 
        ///  ITransformingContext.
        ///  
        ///  See ITransformingContext for more information.
        /// </remarks>
        IMatchingContext CreateMatchingContext(
              object content
            , ContentType contentType
        );
        #endregion
    }
}
