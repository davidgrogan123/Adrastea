using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Text.RegularExpressions;

namespace Nightcap.Adrastea.Core
{
    /// <summary>
    /// Matches a regular expression against a file entry.
    /// </summary>
    public sealed class RegexFileEntryMatcher : IMatcher
    {
        #region constants

        private static class Constants
        {
            internal const string FailureMessage_RegexParseError_1 =
                "Could not parse regular expression. {0}";
        }
        #endregion

        #region fields

        private readonly Regex m_regex;
        #endregion

        #region construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regexString">
        /// The regular expression pattern to match. Must not be <c>null</c>.
        /// </param>
        /// <exception>TBD</exception>
        public RegexFileEntryMatcher(
            string regexString
        )
        {
            Requires.ThatArgumentIsNotNull(regexString, "regexString");

            try
            {
                m_regex = new Regex(regexString);
            }
            catch (System.ArgumentException e)
            {
                // TODO: determine correct exception type
                throw new System.ApplicationException(
                    String.Format(
                          Constants.FailureMessage_RegexParseError_1
                        , e.Message
                    )
                    , e
                );
            }
        }
        #endregion

        #region methods

        /// <inheritdoc/>
        public bool IsMatch(
            IMatchingContext context
        )
        {
            Requires.ThatArgumentIsNotNull(context, "context");

            IFileEntryMatchingContext fileEntry;
            if (context.TryGetInterface(out fileEntry))
            {
                return m_regex.IsMatch(fileEntry.FileEntry);
            }

            return false;
        }
        #endregion
    }
}
