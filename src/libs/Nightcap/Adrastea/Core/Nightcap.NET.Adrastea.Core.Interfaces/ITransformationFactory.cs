using System.Xml;

namespace Nightcap.Adrastea.Core.Interfaces
{
    /// <summary>
    ///  Creates instances of transformations.
    /// </summary>
    public interface ITransformationFactory
    {
        #region methods

        /// <summary>
        ///  Creates a transformation based on the specified config.
        /// </summary>
        /// <param name="config">
        ///  The xml config that determines how the transformation will be
        ///  constructed. May not be <c>null</c>.
        /// </param>
        /// <returns>The newly constructed transformation.</returns>
        /// <exception>TBC</exception>
        ITransformation CreateTransformation(
            XmlReader config
        );
        #endregion
    }
}
