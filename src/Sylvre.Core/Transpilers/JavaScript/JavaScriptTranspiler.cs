using System.Text;
using System.Collections.Generic;

using static SylvreParser;

using Sylvre.Core.Models;
using Sylvre.Core.Transpilers.JavaScript.SylvreLibrary;

namespace Sylvre.Core.Transpilers.JavaScript
{
    /// <summary>
    /// Provides functionality to transpile Sylvre programs into JavaScript.
    /// </summary>
    internal partial class JavaScriptTranspiler : SylvreParserBaseVisitor<object> 
    {
        private StringBuilder _output = new StringBuilder();
        private List<SylvreErrorBase> _transpileErrors = new List<SylvreErrorBase>();

        private readonly static string _sylvreDeclarationDisallowedMessage = "'Sylvre' is a special keyword reserved for the builtin library and cannot be used as a variable name.";
        private readonly static string _sylvreAssignmentDisallowedMessage = "'Sylvre' is a special keyword reserved for the builtin library and cannot be assigned to.";

        private readonly static string _sylvreLibraryIndexReferenceInvalidMessage = "An index reference is not allowed after the library reference.";
        private readonly static string _sylvreModuleIndexReferenceInvalidMessage = "An index reference is not allowed after a module reference.";

        private readonly static string _sylvreMissingModuleMessage = "Missing a module name after the library reference.";
        private readonly static string _sylvreInvalidModuleMessage = "This Sylvre module does not exist.";

        private readonly static string _sylvreMissingModuleMemberMessage = "Missing a module member reference after module name.";
        private readonly static string _sylvreInvalidModuleMemberMessage = "This Sylvre module member does not exist.";

        /// <summary>
        /// Transpile a given Sylvre program into JavaScript.
        /// </summary>
        /// <param name="sylvreProgram">The Sylvre program to transpile.</param>
        /// <returns>The transpiled JavaScript output.</returns>
        public JavaScriptOutput Transpile(SylvreProgram sylvreProgram)
        {
            // using strict mode for JS -
            _output.Append("\"use strict\";");
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Strict_mode

            foreach (BlockContext block in sylvreProgram.SylvreCstRoot.block())
            {
                VisitBlock(block);
            }

            return new JavaScriptOutput(_output.ToString(), _transpileErrors);
        }

        /// <summary>
        /// Checks to see if the given variable_complex_reference is a Sylvre library ref (e.g Sylvre.Console.output).
        /// </summary>
        /// <param name="context">The variable_complex_reference context to check.</param>
        /// <returns>True if is a Sylvre library reference, false otherwise.</returns>
        private bool IsSylvreLibraryReference(Variable_complex_referenceContext context)
        {
            return context.variable_reference().GetText() == "Sylvre";
        }
        /// <summary>
        /// Handles the variable_complex_reference if it is a Sylvre library reference.
        /// </summary>
        /// <param name="context">The variable_complex_reference to handle.</param>
        /// <param name="isFunctionCall">Whether the Sylvre library reference is a function call or not.</param>
        private void HandleSylvreLibraryReference(Variable_complex_referenceContext context, bool isFunctionCall)
        {
            var suffixes = context.variable_suffix();
            if (suffixes == null || suffixes.Length == 0)
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = context.variable_reference().Stop.Column,
                    Line = context.variable_reference().Stop.Line,
                    Symbol = context.variable_reference().Stop.Text,
                    Message = _sylvreMissingModuleMessage
                });

                return;
            }

            var firstSuffixContext = context.variable_suffix()[0];
            if (firstSuffixContext.index_reference() != null)
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = firstSuffixContext.index_reference().Start.Column + 1, // Column number 1 behind, zero based index?
                    Line = firstSuffixContext.index_reference().Start.Line,
                    Symbol = firstSuffixContext.index_reference().Start.Text,
                    Message = _sylvreLibraryIndexReferenceInvalidMessage
                });

                return;
            }

            var moduleContext = firstSuffixContext.member_reference().variable_reference();
            string module = moduleContext.GetText();
            if (!SylvreJavaScriptMappings.DoesSylvreModuleExist(module))
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = moduleContext.Start.Column + 1, // Column number 1 behind, zero based index?
                    Line = moduleContext.Start.Line,
                    Symbol = moduleContext.Start.Text,
                    Message = _sylvreInvalidModuleMessage
                });

                return;
            }

            string jsEquivalentModule = SylvreJavaScriptMappings.GetEquivalentJavaScriptModule(module);
            if (!string.IsNullOrWhiteSpace(jsEquivalentModule))
            {
                _output.Append(jsEquivalentModule)
                       .Append('.');
            }

            if (suffixes.Length <= 1)
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = moduleContext.Stop.Column,
                    Line = moduleContext.Stop.Line,
                    Symbol = moduleContext.Stop.Text,
                    Message = _sylvreMissingModuleMemberMessage
                });

                return;
            }

            var secondSuffixContext = context.variable_suffix()[1];
            if (secondSuffixContext.index_reference() != null)
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = secondSuffixContext.index_reference().Start.Column + 1, // Column number 1 behind, zero based index?
                    Line = secondSuffixContext.index_reference().Start.Line,
                    Symbol = secondSuffixContext.index_reference().Start.Text,
                    Message = _sylvreModuleIndexReferenceInvalidMessage
                });

                return;
            }


            var moduleMemberContext = secondSuffixContext.member_reference().variable_reference();
            string moduleMember = moduleMemberContext.GetText();
            if (!SylvreJavaScriptMappings.DoesSylvreMemberOfModuleExist(module, moduleMember))
            {
                _transpileErrors.Add(new SylvreTranspileError
                {
                    CharPositionInLine = moduleMemberContext.Start.Column + 1, // Column number 1 behind, zero based index?
                    Line = moduleMemberContext.Start.Line,
                    Symbol = moduleMemberContext.Start.Text,
                    Message = _sylvreInvalidModuleMemberMessage
                });

                return;
            }

            _output.Append(SylvreJavaScriptMappings.GetEquivalentJavaScriptMemberOfModule(
                module, moduleMember));

            for (int i = 2; i < context.variable_suffix().Length; i++)
            {
                VisitVariable_suffix(context.variable_suffix()[i]);
            }
        }
    }
}
