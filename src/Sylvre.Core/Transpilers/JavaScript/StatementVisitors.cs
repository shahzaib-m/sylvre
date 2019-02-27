using Antlr4.Runtime.Misc;

using static SylvreParser;

using Sylvre.Core.Models;

namespace Sylvre.Core.Transpilers.JavaScript
{
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object>
    {
        public override object VisitStatement([NotNull] StatementContext context)
        {
            if (context.declaration() != null)
            {
                VisitDeclaration(context.declaration());
            }
            else if (context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
            }
            else if (context.function_return() != null)
            {
                VisitFunction_return(context.function_return());
            }

            return null;
        }

        public override object VisitDeclaration([NotNull] DeclarationContext context)
        {
            // checking if 'Sylvre' is being declared and assigned to. Sylvre is a builtin library so disallowed
            if (context.variable_reference().GetText() == "Sylvre")
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = context.variable_reference().Start.Column + 1, // Column number 1 behind, zero based index?
                    Line = context.variable_reference().Start.Line,
                    Symbol = context.variable_reference().Start.Text,
                    Message = _sylvreDeclarationDisallowedMessage
                });
            }

            _output.Append("var ");
            VisitVariable_reference(context.variable_reference());
            _output.Append(context.EQUALSYMBOL().GetText());

            if (context.conditional_expression() != null)
            {
                VisitConditional_expression(context.conditional_expression());
            }
            else
            {
                VisitArray_assignment(context.array_assignment());
            }

            return null;
        }

        public override object VisitFunction_call([NotNull] Function_callContext context)
        {
            VisitVariable_complex_reference(context.variable_complex_reference());
            _output.Append('(');

            if (context.arguments() != null)
            {
                var conditionalExpressions = context.arguments().conditional_expression();
                for (int i = 0; i < conditionalExpressions.Length; i++)
                {
                    VisitConditional_expression(conditionalExpressions[i]);

                    if (i != conditionalExpressions.Length - 1)
                    {
                        _output.Append(',');
                    }
                }
            }

            _output.Append(')');
            return null;
        }
        public override object VisitFunction_return([NotNull] Function_returnContext context)
        {
            _output.Append("return");

            if (context.function_return_value() != null)
            {
                _output.Append(' ');
                VisitConditional_expression(context.function_return_value().conditional_expression());
            }

            return null;
        }
    }
}