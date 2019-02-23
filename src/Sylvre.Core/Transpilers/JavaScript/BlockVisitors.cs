using System;

using Antlr4.Runtime.Misc;

using static SylvreParser;

namespace Sylvre.Core.Transpilers.JavaScript
{
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object>
    {
        public override object VisitBlock([NotNull] BlockContext context)
        {
            if (context.function_block() != null)
            {
                throw new NotImplementedException();
            }
            else if (context.if_block() != null)
            {
                throw new NotImplementedException();
            }
            else if (context.loopwhile_block() != null)
            {
                throw new NotImplementedException();
            }
            else if (context.loopfor_block() != null)
            {
                throw new NotImplementedException();
            }
            else if (context.statement_block() != null)
            {
                throw new NotImplementedException();
            }

            return null;
        }
    }
}
