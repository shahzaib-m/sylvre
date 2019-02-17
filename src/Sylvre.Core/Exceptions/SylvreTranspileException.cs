using System;
using System.Runtime.Serialization;

namespace Sylvre.Core
{
    [Serializable]
    internal class SylvreTranspileException : Exception
    {
        public SylvreTranspileException()
        {
        }

        public SylvreTranspileException(string message) : base(message)
        {
        }

        public SylvreTranspileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SylvreTranspileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}