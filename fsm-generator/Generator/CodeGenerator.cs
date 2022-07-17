using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Fsm_Generator.Parser;
using Fsm_Generator.DataObjects;
using Stubble.Core;
using Stubble.Core.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fsm_Generator.Generator
{
    public class CodeGenerator
    {
        /// <summary>
        /// The metadata extracted from the PUML text.
        /// </summary>
        private ModelData? _data;

        public String HeaderFilename
        {
            get
            {
                if (_data == null)
                {
                    throw new NullReferenceException("Parser data not ready.  Call CreateTargetCode first.");
                }
                return _data.Namespace + ".h";
            }
        }

        public String BodyFilename
        {
            get
            {
                if (_data == null)
                {
                    throw new NullReferenceException("Parser data not ready.  Call CreateTargetCode first.");
                }
                return _data.Namespace + ".cpp";
            }
        }

        public String Namespace
        {
            get
            {
                if (_data == null)
                {
                    throw new NullReferenceException("Parser data not ready.  Call CreateTargetCode first.");
                }
                return _data.Namespace;
            }
        }

        public CodeGenerator()
        {
        }

        /// <summary>
        /// Loads and parses a PUML diagram in preparation for 
        /// processing an output template.
        /// </summary>
        /// <param name="pumlText"></param>
        public void LoadPumlDiagram(String pumlText)
        {
            PumlLexer lexer = new PumlLexer(new AntlrInputStream(pumlText));
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(new ThrowingErrorListener<int>());

            PumlGrammar parser = new PumlGrammar(new CommonTokenStream(lexer));
            parser.RemoveErrorListeners();
            parser.AddErrorListener(new ThrowingErrorListener<IToken>());

            // Parse the script ready for execution
            IParseTree parseTree = parser.stateModel();

            PumlGrammarVisitor<String> visitor = new Fsm_Generator.Parser.PumlGrammarVisitor<String>();
            string result = visitor.Visit(parseTree);
            _data = visitor.Data;
        }

        public String RenderTemplate(String templateFileName)
        {
            if (_data == null)
            {
                throw new NullReferenceException("Parser data not ready.  Call LoadPumlDiagram first.");
            }

            StubbleVisitorRenderer renderer = new StubbleBuilder().Build();

            using (StreamReader reader = new StreamReader(templateFileName))
            {
                return renderer.Render(reader.ReadToEnd(), _data);
            }
        }
    }
}