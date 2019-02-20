using System.IO;
using System.Collections.Generic;

using Antlr4.Runtime;

using Sylvre.Core.Models;

namespace Sylvre.Core
{
    /// <summary>
    /// Provides the custom implementation of SyntaxError() for when the parser encounters parse errors.
    /// </summary>
    internal class ParserErrorListener : BaseErrorListener
    {
        /// <summary>
        /// The list of all parse errors, empty if no errors encountered (successful parse).
        /// </summary>
        public List<SylvreParseError> ParseErrors { get; private set; } = new List<SylvreParseError>();
        /// <summary>
        /// Returns true if the parser encountered errors, false otherwise.
        /// </summary>
        public bool EncounteredParseErrors {
            get
            {
                return ParseErrors.Count != 0;
            }
        }

        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            ParseErrors.Add(new SylvreParseError
            {
                IsMismatchedInput = msg.StartsWith("mismatched") ? true : false,
                Symbol = offendingSymbol.Text,
                Line = line,
                CharPositionInLine = charPositionInLine,
                Message = msg
            });
        }
    }
}
