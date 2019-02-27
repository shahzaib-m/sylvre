using Antlr4.Runtime.Misc;

using static SylvreParser;

namespace Sylvre.Core.Transpilers.JavaScript
{
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object>
    {
        public override object VisitVariable_reference([NotNull] Variable_referenceContext context)
        {
            bool isVariableNameReserved = JavaScriptReservedKeywords.IsReservedKeyword(
                context.IDENTIFIER().GetText());

            _output.Append(isVariableNameReserved ? "__" : "")  // appending two underscores if the var name is a reserved JS keyword
                   .Append(context.IDENTIFIER().GetText());

            return null;
        }

        public override object VisitArithmetic_operator([NotNull] Arithmetic_operatorContext context)
        {
            if (context.PLUS() != null)
                _output.Append('+');
            else if (context.MINUS() != null)
                _output.Append('-');
            else if (context.MULTIPLY() != null)
                _output.Append('*');
            else if (context.DIVIDE() != null)
                _output.Append('/');

            return null;
        }

        public override object VisitString([NotNull] StringContext context)
        {
            if (context.SINGLE_STRING() != null)
            {
                _output.Append(context.SINGLE_STRING());
            }
            else if (context.DOUBLE_STRING() != null)
            {
                _output.Append(context.DOUBLE_STRING());
            }

            return null;
        }
    }
}
