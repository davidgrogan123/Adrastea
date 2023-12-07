namespace Nightcap.Core.Diagnostics.CodeContract
{
    /// <summary>
    ///  A delegate that defines a set of criteria that must be true.
    /// </summary>
    /// <returns>
    ///  <c>true</c> if the criteria are met, otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    ///  See <see cref="Requires.ThatArgument(Predicate,string,string)"/> or 
    ///  <see cref="Requires.That(Predicate,string)"/> for example use.
    /// </remarks>
    public delegate bool Predicate();
}
