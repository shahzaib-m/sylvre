namespace Sylvre.Core.Models
{
    /// <summary>
    /// Represents an input parse error encountered by the parser.
    /// </summary>
    public class SylvreParseError : SylvreErrorBase
    {
        /// <summary>
        /// Whether the error was a 'mismatched input', as opposed to a 'no viable alternative' error.
        /// </summary>
        public bool IsMismatchedInput { get; set; }
    }
}
