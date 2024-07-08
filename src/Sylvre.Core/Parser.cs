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
        /// <returns>The generated Sylvre program.</returns>
        public static SylvreProgram ParseSylvreInput(string input)
        {
            ICharStream inputStream = CharStreams.fromString(input);
            ITokenSource lexer = new SylvreLexer(inputStream);
            ITokenStream tokens = new CommonTokenStream(lexer);

            var parser = new SylvreParser(tokens);
            parser.RemoveErrorListeners();

            var errorListener = new ParserErrorListener();
            parser.AddErrorListener(errorListener);

            SylvreParser.ProgramContext programContext = parser.program();
            return new SylvreProgram(programContext, errorListener.ParseErrors);
        }
    }
}
