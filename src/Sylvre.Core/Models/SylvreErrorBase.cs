namespace Sylvre.Core.Models
{
    /// <summary>
    /// The base class representing a Sylvre error.
    /// </summary>
    public abstract class SylvreErrorBase
    {
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
