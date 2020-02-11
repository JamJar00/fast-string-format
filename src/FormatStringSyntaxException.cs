using System;
using System.Runtime.Serialization;

namespace FastStringFormat
{
    [Serializable]
    public class FormatStringSyntaxException : Exception
    {
        public FormatStringSyntaxException()
        {
        }

        public FormatStringSyntaxException(string message) : base(message)
        {
        }

        public FormatStringSyntaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FormatStringSyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}