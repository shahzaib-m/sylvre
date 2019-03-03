using System.Collections.Generic;

using Sylvre.Core;
using Sylvre.Core.Models;

namespace Sylvre.WebAPI.Data
{
    /// <summary>
    /// Represents the possible source of errors during the transpiling request.
    /// </summary>
    public enum TranspileResponseErrorSource { None, Parser, Transpiler }
    
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
        public TranspileResponseErrorSource ErrorSource { get; set; } = TranspileResponseErrorSource.None;
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
