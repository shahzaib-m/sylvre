using System.Text;
using System.Collections.Generic;

using static SylvreParser;

using Sylvre.Core.Models;

namespace Sylvre.Core.Transpilers.JavaScript
{
    /// <summary>
    /// Provides functionality to transpile Sylvre programs into JavaScript.
    /// </summary>
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object> 
    {
        private StringBuilder _output = new StringBuilder();
        private List<SylvreTranspileError> _transpileErrors = new List<SylvreTranspileError>();

        /// <summary>
        /// Transpile a given Sylvre program into JavaScript.
        /// </summary>
        /// <param name="sylvreProgram">The Sylvre program to transpile.</param>
        /// <returns>The transpiled JavaScript output.</returns>
        public JavaScriptOutput Transpile(SylvreProgram sylvreProgram)
        {
            // using strict mode for JS -
            _output.Append("\"use strict\";");
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Strict_mode

            foreach (BlockContext block in sylvreProgram.SylvreCstRoot.block())
            {
                VisitBlock(block);
            }

            return new JavaScriptOutput(_output.ToString(), _transpileErrors);
        }
    }
}
