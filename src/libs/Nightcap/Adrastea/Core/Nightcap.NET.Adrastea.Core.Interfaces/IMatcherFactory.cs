using System.Xml;

namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  Creates instances of matchers.
    /// </summary>
    public interface IMatcherFactory
    {
        #region methods

        /// <summary>
        ///  Creates a matcher based on the specified config.
        /// </summary>
        /// <param name="config">
        ///  The xml config that determines how the matcher will be
        ///  constructed. May not be <c>null</c>.
        /// </param>
        /// <returns>The newly constructed matcher.</returns>
        /// <exception>TBC</exception>
        IMatcher CreateMatcher(
            XmlReader config
        );
        #endregion
    }
}
