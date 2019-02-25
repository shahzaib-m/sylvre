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
    }
}