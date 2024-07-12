using Antlr4.Runtime.Misc;

using static SylvreParser;

namespace Sylvre.Core.Transpilers.JavaScript
{
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object>
    {
        public override object VisitVariable_complex_reference([NotNull] Variable_complex_referenceContext context)
        {
            if (IsSylvreLibraryReference(context))
            {
                HandleSylvreLibraryReference(context, false);
            }
            else
            {
                VisitVariable_reference(context.variable_reference());

                if (context.variable_suffix() != null)
                {
                    foreach (var suffix in context.variable_suffix())
                    {
                        VisitVariable_suffix(suffix);
                    }
                }
            }

            return null;
        }
        public override object VisitVariable_complex_reference_left([NotNull] Variable_complex_reference_leftContext context)
        {
            VisitVariable_reference(context.variable_reference());

            if (context.variable_suffix_left() != null)
            {
                foreach (var suffix in context.variable_suffix_left())
                {
                    VisitVariable_suffix_left(suffix);
                }
            }

            return null;
        }

        public override object VisitVariable_suffix([NotNull] Variable_suffixContext context)
        {
            if (context.member_reference() != null)
            {
                VisitMember_reference(context.member_reference());
            }
            else if (context.index_reference() != null)
            {
                VisitIndex_reference(context.index_reference());
            }

            return null;
        }
        public override object VisitVariable_suffix_left([NotNull] Variable_suffix_leftContext context)
        {
            if (context.member_reference_left() != null)
            {
                VisitMember_reference_left(context.member_reference_left());
            }
            else if (context.index_reference() != null)
            {
                VisitIndex_reference(context.index_reference());
            }

            return null;
        }

        public override object VisitMember_reference([NotNull] Member_referenceContext context)
        {
            _output.Append('.');

            if (context.variable_reference() != null)
            {
                VisitVariable_reference(context.variable_reference());
            }
            else if (context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
            }

            return null;
        }
        public override object VisitMember_reference_left([NotNull] Member_reference_leftContext context)
        {
            _output.Append('.');
            VisitVariable_reference(context.variable_reference());

            return null;
        }

        public override object VisitIndex_reference([NotNull] Index_referenceContext context)
        {
            _output.Append('[');
            VisitExpression(context.expression());
            _output.Append(']');

            return null;
        }
    }
}
