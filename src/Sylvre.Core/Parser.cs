using Antlr4.Runtime;

using Sylvre.Core.Models;

namespace Sylvre.Core
{
    /// <summary>
    /// Provides functionality to parse an input and generate a Sylvre program.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses the given input according to Sylvre grammar and generates a Sylvre program.
        /// </summary>
        /// <param name="input">The input Sylvre code to be parsed.</param>
        /// <exception cref="SylvreParseException">
        /// Thrown when parsing input to a Sylvre program failed. "ParseErrors" in Data property contains the relevant errors.
        /// </exception>
        /// <returns>The generated Sylvre program.</returns>
        public static SylvreProgram ParseSylvreInput(string input)
        {
            ICharStream inputStream = CharStreams.fromstring(input);
            ITokenSource lexer = new SylvreLexer(inputStream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            SylvreParser parser = new SylvreParser(tokens);
            parser.RemoveErrorListeners();

            ParserErrorListener errorListener = new ParserErrorListener();
            parser.AddErrorListener(errorListener);

            SylvreParser.ProgramContext programContext = parser.program();
            if (errorListener.EncounteredParseErrors)
            {
                var ex = new SylvreParseException();
                ex.Data.Add("ParseErrors", errorListener.ParseErrors);

                throw ex;
            }

            return new SylvreProgram(programContext);
        }
    }
}
