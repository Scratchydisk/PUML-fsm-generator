using System;
using System.Collections.Generic;
using System.Text;

namespace Fsm_Generator.Parser
{

    [Serializable]
    public class ParserException : Exception
    {
        // Line number of error
        public int line;

        public ParserException() { }
        public ParserException(string message) : base(message) { }
        public ParserException(string message, Exception inner) : base(message, inner) { }
        protected ParserException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public ParserException(string message, int line) : base(message)
        {
            this.line = line;
        }

    }
}
