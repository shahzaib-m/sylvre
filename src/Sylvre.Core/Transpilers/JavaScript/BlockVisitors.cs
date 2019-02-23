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
                VisitFunction_block(context.function_block());
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

        public override object VisitFunction_block([NotNull] Function_blockContext context)
        {
            _output.Append("function ")
                   .Append(context.variable_reference().GetText())
                   .Append('(');

            if (context.parameters() != null)
            {
                var variableReferences = context.parameters().variable_reference();
                for (int i = 0; i < variableReferences.Length; i++)
                {
                    _output.Append(variableReferences[i].GetText());

                    if (i != variableReferences.Length - 1)
                    {
                        _output.Append(',');
                    }
                }
            }

            _output.Append(')')
                   .Append('{');

            foreach (var nestableBlock in context.nestable_block())
            {
                VisitNestable_block(nestableBlock);
            }

            _output.Append('}');

            return null;
        }
    }
}
