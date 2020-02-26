// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UndoCommandBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the undo command base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Plugins.Transactions
{
    using Kephas.Plugins.Transactions;
    using NUnit.Framework;

    [TestFixture]
    public class UndoCommandBaseTest
    {
        [Test]
        public void Parse_move_file()
        {
            var cmd = "move|file1|file2";
            var command = UndoCommandBase.Parse(cmd);

            Assert.IsInstanceOf<MoveFileUndoCommand>(command);
            var moveCommand = (MoveFileUndoCommand)command;
            Assert.AreEqual("file1", moveCommand.SourceFile);
            Assert.AreEqual("file2", moveCommand.TargetFile);
        }

        [Test]
        public void Parse_move_file_specials()
        {
            var cmd = "move|file1&bs;net&bs;replay&bs;this|file1&bs;net&bs;replay&bs;this&amp;that";
            var command = UndoCommandBase.Parse(cmd);

            Assert.IsInstanceOf<MoveFileUndoCommand>(command);
            var moveCommand = (MoveFileUndoCommand)command;
            Assert.AreEqual("file1\\net\\replay\\this", moveCommand.SourceFile);
            Assert.AreEqual("file1\\net\\replay\\this&that", moveCommand.TargetFile);
        }

        [Test]
        public void ToString_move_file()
        {
            var cmd = new MoveFileUndoCommand("file1", "file2").ToString();
            Assert.AreEqual("move|file1|file2", cmd);
        }

        [Test]
        public void ToString_move_file_specials()
        {
            var cmd = new MoveFileUndoCommand("file1\\net\\replay\\this", "file1\\net\\replay\\this&that").ToString();
            Assert.AreEqual("move|file1&bs;net&bs;replay&bs;this|file1&bs;net&bs;replay&bs;this&amp;that", cmd);
        }
    }
}
