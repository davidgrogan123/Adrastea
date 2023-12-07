namespace Nightcap.Core.Diagnostics.CodeContract
{
    /// <summary>
    ///  Specifies the cause of the code contract violation.
    /// </summary>
    public enum ViolationType
    {
        /// <summary>
        ///  Default Value.
        /// </summary>
        None = 1,
        /// <summary>
        ///  The Code contract was violated by a <c>null</c> argument.
        /// </summary>
        ArgumentWasNull,
        /// <summary>
        ///  The Code contract was violated by a <c>null</c> or whitespace 
        ///  only argument.
        /// </summary>
        ArgumentWasNullOrWhitespace,
        /// <summary>
        ///  The code contract was violated by an argument predicate that
        ///  evaluated to false.
        /// </summary>
        ArgumentPredicateWasFalse,
        /// <summary>
        ///  The code contract was violated by a predicate that evaluated
        ///  to false.
        /// </summary>
        PredicateWasFalse,
    }
}
