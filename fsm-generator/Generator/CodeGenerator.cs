using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Fsm_Generator.Parser;
using Fsm_Generator.DataObjects;
using Stubble.Core;
using Stubble.Core.Exceptions;
using Stubble.Core.Builders;
using Stubble.Helpers.Builders;
using Stubble.Helpers;
using Stubble.Helpers.Contexts;
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

        public String DiagramFileName
        {
            get;
            set;
        }

        public String TemplateFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Command line args, to be merged to _data
        /// </summary>
        private ProgramMetadata _metadata;


        public CodeGenerator(ProgramMetadata metadata)
        {
            _metadata = metadata;
            DiagramFileName = "";
            TemplateFileName = "";
        }

        /// <summary>
        /// Carries out post processing (mapping events/states)
        /// for the parsed diagram.
        /// </summary>
        /// <param name="data"></param>
        private void PostProcess(ModelData data)
        {
            // Get all the source states for each event
            foreach (EventDto e in data.Events)
            {
                e.SourceStates = data.States.Where(
                    s => data.Transitions.Where(
                        t => t.EventName == e.EventName
                    ).Select(t => t.StartStateName)
                    .Contains(s.StateName)
                )
                .ToList();

                e.TargetState = data.States.Where(
                    s => data.Transitions.Where(
                        t => t.EventName == e.EventName
                    ).Select(t => t.EndStateName)
                    .Contains(s.StateName)
                )
                .First();

                e.TransitionDescriptions = data.Transitions
                .Where(t => t.EventName == e.EventName)
                .SelectMany(t => t.Description)
                .ToList();
            }
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

            PostProcess(_data);
        }

        public String RenderTemplate()
        {
            TemplateFileName = _metadata.TemplateFileName;
            if (_data == null)
            {
                throw new NullReferenceException("Parser data not ready.  Call LoadPumlDiagram first.");
            }

            var helpers = new HelpersBuilder()
                .Register<string>("Upper", (context, arg) =>
                {
                    return arg.ToUpperInvariant();
                });

            var renderer = new StubbleBuilder()
                .Configure(conf => conf.AddHelpers(helpers))
                .Build();

            _data.Metadata = _metadata;
            _data.F = new StubbleFuncs();

            try
            {
                using (StreamReader reader = new StreamReader(TemplateFileName))
                {
                    return renderer.Render(reader.ReadToEnd(), _data);
                }
            }
            catch (StubbleException se)
            {
                return $"Error: {se.Message}";
            }
        }
    }
}