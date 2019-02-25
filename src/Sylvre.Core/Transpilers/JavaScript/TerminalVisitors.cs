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
    }
}
