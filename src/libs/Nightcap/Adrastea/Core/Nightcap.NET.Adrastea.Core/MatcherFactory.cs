using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    ///  Creates instances of matchers.
    /// </summary>
    /// <remarks>
    ///  TODO: Consider caching matchers.
    /// </remarks>
    public sealed class MatcherFactory : IMatcherFactory
    {
        #region constants

        private static class Constants
        {
            internal const string FailureMessage_TypeNotFound_3 =
                "Could not create a matcher type of '{0}' from attribute '{1}' in "
                + "element '{2}'. This type doesn't match any of the pre-configured "
                + "types";

            public static class XmlAttributes
            {
                internal const string Type = "type";
            }

            public static class XmlElements
            {
                internal const string Status = "status";
            }

            public static class TypeName
            {
                internal const string RegexFileEntry = "RegexFileEntryMatcher";
            }
        }
        #endregion

        #region methods

        private
            IMatcher
            CreateRegexFileEntryMatcher(
                  XElement elm
            )
        {
            return new RegexFileEntryMatcher(
                  elm.Value
            );
        }

        /// <inheritdoc/>
        public
            IMatcher
            CreateMatcher(
                  XmlReader config
            )
        {
            Requires.ThatArgumentIsNotNull(config, "config");

            var elm = XElement.Load(
                  config
            );

            // retrieve the type of matcher to create
            var typeName = XmlUtilities.GetAttributeByName(
                  elm
                , Constants.XmlAttributes.Type
            ).Value;

            // create the new matcher
            switch (typeName)
            {
                case Constants.TypeName.RegexFileEntry:
                    return CreateRegexFileEntryMatcher(
                          elm
                    );

                default:
                    // TODO: determine correct exception type
                    throw new ApplicationException(
                          String.Format(
                                Constants.FailureMessage_TypeNotFound_3
                              , typeName
                              , Constants.XmlAttributes.Type
                              , elm
                          )
                    );
            }
        }
        #endregion
    }
}
