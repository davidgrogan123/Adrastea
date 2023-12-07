using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  A static utility class that provides support methods for operating
    ///  on transformations.
    /// </summary>
    public static class TransformationUtil
    {
        #region implementation

        /// <summary>
        ///  Executes each transformation in <paramref name="transformations"/>
        ///  using the output of the previous transformation as the input.
        /// </summary>
        /// <param name="transformations">
        ///  A collection of transformations to be executed. May not be 
        ///  <c>null</c> or empty.
        /// </param>
        /// <param name="context">
        ///  The initial context, to be used as the input to the first 
        ///  transformation, containing the data to be transformed. May not 
        ///  be <c>null</c>.
        /// </param>
        public static ITransformingContext ExecuteTransformations(
              IEnumerable<ITransformation> transformations
            , ITransformingContext context
            , IEnumerable<Delegate> eventHandlers
        )
        {
            Requires.ThatArgumentIsNotNull(transformations, "IEnumerable<ITransformation>");
            Requires.ThatArgumentIsNotNull(context, "ITransformingContext");
            Requires.ThatArgument(() => transformations.Any(), "transformations", "Transformations may not be empty");

            ITransformingContext contextOut = context;

            foreach (ITransformation t in transformations)
            {
                using (ITransformingContext contextIn = contextOut)
                {
                    contextOut = t.Transform(contextIn, eventHandlers);
                }
            }

            return contextOut;
        }
        #endregion
    }
}
