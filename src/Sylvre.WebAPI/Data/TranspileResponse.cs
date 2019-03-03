using System.Collections.Generic;

using Sylvre.Core;
using Sylvre.Core.Models;

namespace Sylvre.WebAPI.Data
{
    public enum TranspileResponseErrorSource { None, Parser, Transpiler, }

    public class TranspileResponse
    {
        public bool HasErrors { get; set; } = false;
        public TranspileResponseErrorSource ErrorSource { get; set; } = TranspileResponseErrorSource.None;
        public ICollection<SylvreErrorBase> Errors { get; set; }
        
        public TargetLanguage Target { get; set; }
        public string TranspiledCode { get; set; }
    }
}
