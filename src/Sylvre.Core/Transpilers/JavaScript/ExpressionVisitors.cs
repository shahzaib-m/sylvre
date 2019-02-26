using System;

using Antlr4.Runtime.Misc;

using static SylvreParser;

namespace Sylvre.Core.Transpilers.JavaScript
{
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object>
    {
        public override object VisitConditional_expression([NotNull] Conditional_expressionContext context)
        {
            if (context.OPEN_PARENTHESIS() != null)
            {
                _output.Append('(');
                VisitConditional_expression(context.conditional_expression()[0]);
                _output.Append(')');
            }
            else if (context.NOT() != null)
            {
                _output.Append('!');
                VisitConditional_expression(context.conditional_expression()[0]);
            }
            else if (context.comparison_operator() != null)
            {
                VisitConditional_expression(context.conditional_expression()[0]);

                var op = context.comparison_operator();
                if (op.GREATER_THAN() != null)
                    _output.Append('>');
                else if (op.GREATER_EQUAL() != null)
                    _output.Append(">=");
                else if (op.LESS_THAN() != null)
                    _output.Append('<');
                else if (op.LESS_EQUAL() != null)
                    _output.Append("<=");
                else if (op.EQUALS() != null)
                    _output.Append("==");

                VisitConditional_expression(context.conditional_expression()[1]);
            }
            else if (context.logical_operator() != null)
            {
                VisitConditional_expression(context.conditional_expression()[0]);

                var op = context.logical_operator();
                if (op.AND() != null)
                    _output.Append("&&");
                else if (op.OR() != null)
                    _output.Append("||");

                VisitConditional_expression(context.conditional_expression()[1]);
            }
            else
            {
                VisitExpression(context.expression());
            }

            return null;
        }
    }
}
