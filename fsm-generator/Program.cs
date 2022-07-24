using System;
using System.Diagnostics;
using System.IO;
using Fsm_Generator.DataObjects;
using Fsm_Generator.Generator;
using Fsm_Generator.Parser;
using Fsm_Generator.Utils;
using Stubble.Core.Builders;

namespace Fsm_Generator
{
    enum ExitCode : int
    {
        Success = 0,
        NoPumlFileSpecified = 1,
        PumlFileNotFound = 2,
        ErrorReadingPumlFile = 3,
        PumlFileParseError = 4
    }

    class Program
    {
        private static ProgramMetadata _metadata = null!;
        private static CommandLineArgs _arguments = null!;


        static void Main(string[] args)
        {
            Environment.ExitCode = (int)ExitCode.Success;

            _arguments = new CommandLineArgs(args);
            _metadata = new ProgramMetadata(_arguments);

            bool valid = ProcessArguments();

            if (!_metadata.Quiet)
            {
                Console.WriteLine("Grotsoft Finite State Model Generator");
                Console.WriteLine("www.grotsoft.com\r\n");
            }

            if (valid)
            {
                ParseAndGenerate();
            }
        }

        /// <summary>
        /// Returns true if there were no errors
        /// reading the parameters.
        /// </summary>
        /// <returns></returns>
        static bool ProcessArguments()
        {
            bool valid = true;

            if (_arguments["h"] != null)
            {
                PrintCommandLineArgs();
                Environment.Exit((int)ExitCode.Success);
            }

            if (_arguments["tags"] != null)
            {
                PrintTemplateTags();
                Environment.Exit((int)ExitCode.Success);
            }

            if (_arguments["s"] == null)
            {
                Console.Error.WriteLine("State Model option (-s) must be supplied.");
                Environment.ExitCode = (int)ExitCode.NoPumlFileSpecified;
                valid = false;
            }
            else
            {
                _metadata.StateModelFile = _arguments["s"]!;
            }

            if (_arguments["q"] != null)
            {
                _metadata.Quiet = true;
            }

            if (_arguments["t"] != null)
            {
                _metadata.TemplateFileName = _arguments["t"] ?? "";
            }

            // if (_arguments["HelpMerge"] != null)
            // {
            //     PrintMergeHelp();
            // }
            // if (_arguments["ShowMerge"] != null)
            // {
            //     PrintMergeSettings();
            // }
            // if (_arguments["UseP4"] != null)
            // {
            //     if (!bool.TryParse(_arguments["UseP4"], out _metadata.UseP4Merge))
            //     {
            //         Console.Error.WriteLine($"Error: '{_arguments["UseP4"]}' is not a valid boolean value.\r\n");
            //         valid = false;
            //     }
            // }
            // if (_arguments["Merge"] != null)
            // {
            //     if (!bool.TryParse(_arguments["Merge"], out _metadata.Merge))
            //     {
            //         Console.Error.WriteLine($"Error: '{_arguments["Merge"]}' is not a valid boolean value.\r\n");
            //         valid = false;
            //     }
            // }
            if (_arguments["v"] != null)
            {
                _metadata.Verbose = true;
            }

            return valid;
        }

        static String ReadPumlFile(String fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    return File.ReadAllText(fileName);
                }
                else
                {
                    Console.Error.WriteLine($"State model does not exist: {fileName}");
                    Environment.Exit((int)ExitCode.PumlFileNotFound);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading model: {ex.Message}");
                Environment.Exit((int)ExitCode.ErrorReadingPumlFile);
            }
            return string.Empty;
        }

        static void ParseAndGenerate()
        {
            Verbose($"Reading state model {_metadata.StateModelFile}");
            String content = ReadPumlFile(_metadata.StateModelFile);
            if (String.IsNullOrEmpty(content))
            {
                Verbose("Unable to read model, finishing.");
                Environment.Exit((int)ExitCode.ErrorReadingPumlFile);
            }

            Verbose("Creating parser");
            CodeGenerator gen = new CodeGenerator(_metadata);

            Verbose("Parsing model");
            try
            {
                gen.LoadPumlDiagram(content);
            }
            catch (ParserException ex)
            {
                WriteParserError(ex, content);
                Environment.Exit((int)ExitCode.PumlFileParseError);
            }

            _metadata.Namespace = gen.Namespace;
            Verbose("Parse complete.");


            if (string.IsNullOrEmpty(_metadata.TemplateFileName))
            {
                Console.WriteLine("No merge template specified, exiting.");
                Environment.Exit((int)ExitCode.Success);
            }
            Verbose("Rendering Template");
            String outputStr = gen.RenderTemplate();

            // TODO: Option for an output file
            Console.WriteLine(outputStr);

            Verbose("Done");
        }


        /// <summary>
        /// Writes the text to the console if verbose mode is on.
        /// </summary>
        /// <param name="text"></param>
        static void Verbose(string text)
        {
            if (_metadata.Verbose)
                Console.WriteLine(text);
        }

        static void PrintCommandLineArgs()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("fsm-generator -s input.puml [options]");
            //Console.WriteLine("\r\nDefault values shown in []");
            Console.WriteLine(
                "\r\nOptions:\r\n"
                + "\t-h\t\tShow this help\r\n"
                + "\t-o <file name>\tOutput file, optional\r\n"
                + "\t-q\t\tQuiet, supresses program info to stdout\r\n"
                + "\t-s <file name>\tPUML state model to parse (mandatory)\r\n"
                + "\t-t <file name>\tMerge template file\r\n"
                + "\t-tags\t\tShow available template tags\r\n"
                //+ "\tHelpMerge\tShow more details on code merging.\r\n"
                //+ "\tShowMerge\tShow settings for external merge tool.\r\n"
                //+ "\tMerge ([true]|false)\tAttempt to merge generated files with user files.\r\n"
                //+ "\tUseP4 ([true]|false)\tUse P4Merge if internal merge fails.\r\n"
                //+ "\tP4Path P4Path\tSpecify the path to P4Merge.exe.\r\n"
                + "\t-v\t\tVerbose output to console\r\n"
                + "\t\r\n"
                + "To pipeline generated output to stdout:\r\n"
                + "\t\r\n"
                + "\tUse -q option and do not specify an output file\r\n"
                + "\t\r\n"
                + "Exit Codes:\r\n"
                + "\tSuccess = 0\r\n"
                + "\tNoPumlFileSpecified = 1\r\n"
                + "\tPumlFileNotFound = 2\r\n"
                + "\tErrorReadingPumlFile = 3\r\n"
                + "\tPumlFileParseError = 4\r\n"
                );
        }

        static void PrintTemplateTags()
        {
            Console.WriteLine(
                "\r\nTemplate Tags:\r\n"
                + "\t{{DiagramName}}\tDiagram name from the @startuml line\r\n"
                + "\t{{Namespace}}\tDiagram name with whitespace removed\r\n"
                + "\t{{GeneratedOn}}\tTimestamp of template merge\r\n"
                + "\r\n"
                + "Collections:\r\n"
                + "\t{{Description}}\tCollection of description lines (comments)\r\n"
                + "\t\t{{DescriptionLine}}\tIndividual line of description block\r\n"
                + "\r\n"
                + "\t{{Events}}\tCollection of all model events\r\n"
                + "\r\n"
                + "\t{{States}}\tCollection of model states\r\n"
                + "\t\t{{StateName}}\tName of current state\r\n"
                + "\t\t{{Timeout}}\tTimeout (ms) of current state\r\n"
                + "\r\n"
                + "\t{{Transitions}}\tCollection of state transitions\r\n"
                + "\t\t{{StartStateName}}\tTransition's from state\r\n"
                + "\t\t{{EndStateName}}\tTransition's to state\r\n"
                + "\t\t{{EventName}}\tEvent that triggers transition\r\n"
                + "\r\n"
                + "\t{{UserEvents}}\tCollection of non system events\r\n"
                + "\t\tSystem events are Start, Timeout, OnEntry, OnExit\r\n"
                + "\r\n"
                + "Event Properties (used inside Event collections)\r\n"
                + "\t{{EventDescription}}\tDescription of the event\r\n"
                + "\t{{EventIndex}}\tUnique index number of the event\r\n"
                + "\t{{EventName}}\tName of the event\r\n"
                + "\r\n"
                + "\r\nWorking with Collections:\r\n"
                + "\t{{#Collection}}\tStart of for each loop\r\n"
                + "\t{{/Collection}}\tEnd of for each loop\r\n"
            );
        }

        static void PrintMergeHelp()
        {
            Console.WriteLine("Merge help - TBD\r\n");
        }
        static void PrintMergeSettings()
        {
            Console.WriteLine("Merge Tool Settings:");
            Console.WriteLine(
                $"\r\n\tUse P4Merge:\t{_metadata.UseP4Merge}\r\n"
                + $"\tP4Merge Path:\t{_metadata.P4MergePath}\r\n"
                + $"\tGeneration Folder:\t{_metadata.GenerateFolder}\r\n"
                );
        }

        static void WriteParserError(ParserException pe, String pumlContent)
        {
            ConsoleColor currentFg = Console.ForegroundColor;

            Console.Error.WriteLine($"Error parsing {_metadata.StateModelFile}");
            Console.Error.WriteLine(pe.Message);

            // Show 2 lines before and 2 after the error line
            String[] lines = pumlContent.Split("\n");
            int numLines = lines.Length;

            for (int i = pe.line - 3; i <= pe.line + 3; i++)
            {
                if (i > 0 && i <= numLines)
                {
                    if (i == pe.line)
                        Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Error.WriteLine($"{i:0000}: {lines[i - 1]}");

                    Console.ForegroundColor = currentFg;
                }
            }
        }
    }


}