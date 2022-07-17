using Fsm_Generator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fsm_Generator.DataObjects
{
    /// <summary>
    /// Metadata used to control the generation.
    /// Has defaults which may be overridden by 
    /// command line args.
    /// </summary>
    public class ProgramMetadata
    {
        // Properties set by command line
        public bool Merge;
        public bool UseP4Merge;
        public String P4MergePath;

        public bool UseDiffMerge;
        public String DiffMergePath;

        public bool UseWinMerge;
        public String WinMergePath;

        // For later
        public bool UseCustomMerge;
        public String CustomMergePath;
        public String CustomMergeArguments;

        public String GenerateFolder;
        public String StateModelFile = null!;
        public bool Verbose;
        public String BaseFileRoot;
        public String UserFileRoot;

        // Properties set by parser or generator
        public String BaseFileName;
        public String LeftFileName;
        public String RightFileName;
        public String Namespace;

        public String QualifiedGenerateFolder
        {
            get { return Path.GetFullPath(GenerateFolder) + "\\"; }
        }

        public String QualifiedBaseFileName
        {
            get { return QualifiedGenerateFolder + BaseFileName; }
        }

        public String QualifiedLeftFileName
        {
            get { return QualifiedGenerateFolder + LeftFileName; }
        }

        public String QualifiedRightFileName
        {
            get { return QualifiedGenerateFolder + RightFileName; }
        }

        public ProgramMetadata(CommandLineArgs args)
        {
            Merge = true;
            UseDiffMerge = false;
            UseWinMerge = false;
            P4MergePath = @"%ProgramFiles%\Perforce\p4merge.exe";
            UseP4Merge = true;
            GenerateFolder = ".";
            Verbose = false;
            BaseFileRoot = "mergeBase";
            UserFileRoot = "user";

            DiffMergePath = "";
            WinMergePath = "";
            CustomMergePath = "";
            CustomMergeArguments = "";
            StateModelFile = "";
            BaseFileName = "";
            LeftFileName = "";
            RightFileName = "";
            Namespace = "Default_Name_Space";
        }

        /// <summary>
        /// Prefix filename with the value of
        /// the GenerateFolder attribute.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public String GetPath(String filename)
        {
            String filePath = $@"{GenerateFolder}\{filename}";
            //if(filePath.Contains(" "))
            //    filePath = "\"" + filePath + "\"";

            return filePath;
        }

        /// <summary>
        /// Set the filename attributes for merging for
        /// the supplied extension (.h or .cpp)
        /// </summary>
        /// <param name="extension"></param>
        public void SetMergeFileNames(String extension)
        {
            BaseFileName = GetPath(BaseFileRoot + extension);
            RightFileName = GetPath(UserFileRoot + extension);
            LeftFileName = GetPath("generated" + extension);
        }
    }
}
