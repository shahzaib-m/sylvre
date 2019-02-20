using System;
using System.Runtime.Serialization;

namespace Sylvre.Core
{
    [Serializable]
    public class SylvreParseException : Exception
    {
        public SylvreParseException()
        {
        }

        public SylvreParseException(string message) : base(message)
        {
        }

        public SylvreParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SylvreParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
