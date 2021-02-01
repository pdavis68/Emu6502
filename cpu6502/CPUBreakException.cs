using System;
using System.Runtime.Serialization;

namespace cpu6502
{
    [Serializable]
    public class CPUBreakException : Exception
    {
        public CPUBreakException() { }
        public CPUBreakException(string message) : base(message) { }
        public CPUBreakException(string message, Exception inner) : base(message, inner) { }
        protected CPUBreakException(
          SerializationInfo info,
          StreamingContext context) : base(info, context) { }
    }
}
