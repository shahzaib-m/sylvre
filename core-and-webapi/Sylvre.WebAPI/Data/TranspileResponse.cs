using System.Collections.Generic;

using Sylvre.Core;
using Sylvre.Core.Models;

using Sylvre.WebAPI.Data.Enums;

namespace Sylvre.WebAPI.Data
{
    /// <summary>
    /// Represents the transpile response sent back to the client after a transpile request.
    /// </summary>
    public class TranspileResponse
    {
        /// <summary>
        /// Whether the transpiled result has any errors.
        /// </summary>
        public bool HasErrors { get; set; } = false;
        /// <summary>
        /// The source of the errors, either by the parser during parsing, or by the transpiler after parsing.
        /// </summary>
        public TranspileResponseErrorType ErrorSource { get; set; } = TranspileResponseErrorType.None;
        /// <summary>
        /// The list of all errors.
        /// </summary>
        public ICollection<SylvreErrorBase> Errors { get; set; }
        
        /// <summary>
        /// The target language of the transpiled code.
        /// </summary>
        public TargetLanguage Target { get; set; }
        /// <summary>
        /// The transpiled code.
        /// </summary>
        public string TranspiledCode { get; set; }
    }
}
