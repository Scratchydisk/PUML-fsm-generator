using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fsm_Generator.Parser
{
    public class ThrowingErrorListener<TSymbol> : IAntlrErrorListener<TSymbol>
    {
        
        public void  SyntaxError(TextWriter output, IRecognizer recognizer, TSymbol offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            throw new ParserException($"line {line}:{charPositionInLine} {msg}", line);
        }
    }
}
