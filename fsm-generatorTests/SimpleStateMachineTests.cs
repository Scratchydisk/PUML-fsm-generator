using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Fsm_Generator.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stubble.Core;
using Stubble.Core.Builders;

namespace Fsm_GeneratorTests
{
    [TestClass]
    public class SimpleStateMachineTests
    {
        public string ReadPumlFile(string fileName)
        {
            string path = $"./TestDiagrams/{fileName}";
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            Assert.Fail($"Failed to read test file {path}");
            return null;
        }

        [TestMethod]
        public void TestTheTest()
        {
            Assert.AreEqual(1, 1, "Test failed");
        }

        [TestMethod]
        public void BuiltinBlinkScript()
        {
            string content = ReadPumlFile("BuiltinBlink-FSM.puml");

            CodeGenerator gen = new CodeGenerator();
            gen.CreateTargetCode(content);

            if (File.Exists(gen.HeaderFilename))
            {
                if (File.Exists("gen1.h"))
                    File.Delete("gen1.h");

                File.Move(gen.HeaderFilename, "gen1.h");
            }
            else
            {
                File.WriteAllText("gen1.h", gen.GeneratedHeader);
            }
            File.WriteAllText(gen.HeaderFilename, gen.GeneratedHeader);

            String gen1 = File.ReadAllText("gen1.h");
            String userCode = File.ReadAllText("userCode.h");

            // var merge = Merge.Perform(
            //     gen1.ToCharArray(),
            //     userCode.ToCharArray(),
            //     gen.GeneratedHeader.ToCharArray(),
            //     new BasicReplaceInsertDeleteDiffElementAligner<char>(),
            //     //new BasicInsertDeleteDiffElementAligner<char>(),
            //     //new TakeRightThenLeftIfLeftDiffersFromRightMergeConflictResolver<char>());
            //     new TakeLeftThenRightIfRightDiffersFromLeftMergeConflictResolver<char>());

            // //new TakeLeftMergeConflictResolver<char>());

            // String merged = String.Concat(merge);

            // File.WriteAllText("merged.h", merged);
        }

        [TestMethod]
        public void DiffTest()
        {
            String common = File.ReadAllText("gen1.h");
            DateTime commonDt = File.GetLastWriteTimeUtc("gen1.h");
            String newGen = File.ReadAllText("BuiltInBlinkStateModel.h");
            DateTime newGenDt = File.GetLastWriteTimeUtc("BuiltInBlinkStateModel.h");
            String user = File.ReadAllText("userCode.h");
            DateTime userDt = File.GetLastWriteTimeUtc("userCode.h");

            String left = newGen;
            String right = user;
            //Console.WriteLine($"User date {userDt.ToString()}");
            //Console.WriteLine($"gen date {newGenDt.ToString()}");
            //if (newGenDt < userDt)
            //{
            //    Console.WriteLine("User code is left");
            //    left = user;
            //    right = newGen;
            //}

            // var merge = Merge.Perform<char>(
            //     common.ToCharArray(),
            //     left.ToCharArray(),
            //     right.ToCharArray(),
            //     //new BasicInsertDeleteDiffElementAligner<char>(),
            //     new BasicReplaceInsertDeleteDiffElementAligner<char>(),
            //     //new TakeRightMergeConflictResolver <char>());
            //     //new TakeRightThenLeftIfLeftDiffersFromRightMergeConflictResolver<char>());
            //     new TakeLeftThenRightIfRightDiffersFromLeftMergeConflictResolver<char>());

            // String merged = String.Concat(merge);

            // File.WriteAllText("merged.h", merged);
        }
    }
}