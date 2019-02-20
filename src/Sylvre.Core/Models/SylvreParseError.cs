namespace Sylvre.Core.Models
{
    /// <summary>
    /// Represents an input parse error encountered by the parser.
    /// </summary>
    public class SylvreParseError
    {
        /// <summary>
        /// Whether the error was a 'mismatched input', as opposed to a 'no viable alternative' error.
        /// </summary>
        public bool IsMismatchedInput { get; set; }
        /// <summary>
        /// The exact symbol that caused the error.
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// The line number of the error in the original input.
        /// </summary>
        public int Line { get; set; }
        /// <summary>
        /// The character position in the line number of the error in the original input.
        /// </summary>
        public int CharPositionInLine { get; set; }
        /// <summary>
        /// The error message detailing the parse error.
        /// </summary>
        public string Message { get; set; }
    }
}
