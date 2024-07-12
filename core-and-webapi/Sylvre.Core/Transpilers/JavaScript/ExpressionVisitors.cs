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

        public override object VisitExpression([NotNull] ExpressionContext context)
        {
            if (context.arithmetic_operator() != null)
            {
                VisitTerm(context.term());
                VisitArithmetic_operator(context.arithmetic_operator());
                VisitExpression(context.expression());
            }
            else
            {
                VisitTerm(context.term());
            }

            return null;
        }
        public override object VisitTerm([NotNull] TermContext context)
        {
            if (context.arithmetic_operator() != null)
            {
                VisitFactor(context.factor());
                VisitArithmetic_operator(context.arithmetic_operator());
                VisitTerm(context.term());
            }
            else
            {
                VisitFactor(context.factor());
            }

            return null;
        }
        public override object VisitFactor([NotNull] FactorContext context)
        {
            if (context.expression() != null)
            {
                _output.Append('(');
                VisitExpression(context.expression());
                _output.Append(')');
            }
            else if (context.NUMBER() != null)
            {
                if (context.MINUS() != null)
                {
                    _output.Append('-');
                }

                _output.Append(context.NUMBER().GetText());
            }
            else if (context.DECIMAL() != null)
            {
                if (context.MINUS() != null)
                {
                    _output.Append('-');
                }

                _output.Append(context.DECIMAL().GetText());
            }
            else if (context.@string() != null)
            {
                VisitString(context.@string());
            }
            else if (context.variable_complex_reference() != null)
            {
                VisitVariable_complex_reference(context.variable_complex_reference());
            }
            else if (context.function_call() != null)
            {
                VisitFunction_call(context.function_call());
            }
            else if (context.@bool() != null)
            {
                VisitBool(context.@bool());
            }
            else if (context.unary_increment() != null)
            {
                VisitUnary_increment(context.unary_increment());
            }
            else if (context.unary_decrement() != null)
            {
                VisitUnary_decrement(context.unary_decrement());
            }

            return null;
        }
    }
}
