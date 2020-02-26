// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MoveFileUndoCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the rename undo command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Kephas.Plugins.Transactions
{
    using System.IO;

    /// <summary>
    /// A rename undo command.
    /// </summary>
    public class MoveFileUndoCommand : UndoCommandBase
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        public const string CommandName = "move";

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFileUndoCommand"/> class.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="targetFile">The target file.</param>
        public MoveFileUndoCommand(string sourceFile, string targetFile)
            : base(CommandName, new string[] { sourceFile, targetFile })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFileUndoCommand"/> class.
        /// </summary>
        /// <param name="args">A variable-length parameters list containing arguments.</param>
        public MoveFileUndoCommand(params string[] args)
            : base(CommandName, args)
        {
        }

        /// <summary>
        /// Gets the original file.
        /// </summary>
        /// <value>
        /// The original file.
        /// </value>
        public string SourceFile => this.Args[0];

        /// <summary>
        /// Gets the renamed file.
        /// </summary>
        /// <value>
        /// The renamed file.
        /// </value>
        public string TargetFile => this.Args[1];

        /// <summary>
        /// Executes the undo command.
        /// </summary>
        /// <param name="context">The context.</param>
        public override void Execute(IPluginContext context)
        {
            File.Move(this.TargetFile, this.SourceFile);
        }
    }
}
