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
                VisitIf_block(context.if_block());

                if (context.elseif_block() != null)
                {
                    foreach (var elseifBlock in context.elseif_block())
                    {
                        VisitElseif_block(elseifBlock);
                    }
                }

                if (context.else_block() != null)
                {
                    VisitElse_block(context.else_block());
                }

                return null;

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

        public override object VisitIf_block([NotNull] If_blockContext context)
        {
            _output.Append("if(");

            VisitConditional_expression(context.conditional_expression());

            _output.Append(')')
                   .Append('{');

            foreach (var nestableBlock in context.nestable_block())
            {
                VisitNestable_block(nestableBlock);
            }

            _output.Append('}');

            return null;
        }
        public override object VisitElseif_block([NotNull] Elseif_blockContext context)
        {
            _output.Append("else if(");

            VisitConditional_expression(context.conditional_expression());

            _output.Append(')')
                   .Append('{');

            foreach (var nestableBlock in context.nestable_block())
            {
                VisitNestable_block(nestableBlock);
            }

            _output.Append('}');

            return null;
        }
        public override object VisitElse_block([NotNull] Else_blockContext context)
        {
            _output.Append("else{");

            foreach (var nestableBlock in context.nestable_block())
            {
                VisitNestable_block(nestableBlock);
            }

            _output.Append('}');

            return null;
        }
    }
}
