// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UndoCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the undo command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Plugins.Transactions
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Kephas.Reflection;

    /// <summary>
    /// An undo command base.
    /// </summary>
    public abstract class UndoCommandBase
    {
        /// <summary>
        /// The key part in log of the undo command.
        /// </summary>
        public const string KeyPart = "-undo-";

        /// <summary>
        /// The command activators.
        /// </summary>
        protected static readonly ConcurrentDictionary<string, Func<string[], IPluginContext, UndoCommandBase>> Activators =
            new ConcurrentDictionary<string, Func<string[], IPluginContext, UndoCommandBase>>();

        private const char SplitSeparatorChar = '|';

        private static int index = 0;

        /// <summary>
        /// Initializes static members of the <see cref="UndoCommandBase"/> class.
        /// </summary>
        static UndoCommandBase()
        {
            Activators.TryAdd(MoveFileUndoCommand.CommandName, (args, ctx) => new MoveFileUndoCommand(args));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoCommandBase"/> class.
        /// </summary>
        /// <param name="name">The command name.</param>
        /// <param name="args">The command arguments.</param>
        public UndoCommandBase(string name, params string[] args)
        {
            this.Index = Interlocked.Increment(ref index);
            this.Name = name;
            this.Args = args;

            Activators.TryAdd(name, (ctorArgs, ctx) => (UndoCommandBase)this.GetType().AsRuntimeTypeInfo(ctx?.AmbientServices?.TypeRegistry).CreateInstance(new object[] { ctorArgs }));
        }

        /// <summary>
        /// Gets the command name.
        /// </summary>
        /// <value>
        /// The command name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the command arguments.
        /// </summary>
        /// <value>
        /// The command arguments.
        /// </value>
        public string[] Args { get; }

        /// <summary>
        /// Gets or sets the zero-based index of this object.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; protected internal set; }

        /// <summary>
        /// Parses the command string to restore an <see cref="UndoCommandBase"/> object.
        /// </summary>
        /// <param name="commandString">The command string.</param>
        /// <param name="context">The execution context.</param>
        /// <returns>
        /// An <see cref="UndoCommandBase"/>.
        /// </returns>
        public static UndoCommandBase Parse(string commandString, IPluginContext context)
        {
            var splits = commandString.Split(SplitSeparatorChar);
            var activator = Activators[splits[0]];
            return activator(
                splits.Skip(1)
                    .Select(Unescape)
                    .ToArray(),
                context);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.Name).Append(SplitSeparatorChar);
            foreach (var arg in this.Args)
            {
                sb.Append(Escape(arg)).Append(SplitSeparatorChar);
            }

            sb.Length = sb.Length - 1;

            return sb.ToString();
        }

        /// <summary>
        /// Executes the undo command.
        /// </summary>
        /// <param name="context">The context.</param>
        public abstract void Execute(IPluginContext context);

        private static string Escape(string value)
        {
            var sb = new StringBuilder(value)
                .Replace("&", "&amp;")
                .Replace("\\", "&bs;")
                .Replace(SplitSeparatorChar.ToString(), "&pipe;")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
            return sb.ToString();
        }

        private static string Unescape(string escapedValue)
        {
            var sb = new StringBuilder(escapedValue)
                .Replace("\\t", "\t")
                .Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("&pipe;", SplitSeparatorChar.ToString())
                .Replace("&bs;", "\\")
                .Replace("&amp;", "&");
            return sb.ToString();
        }
    }
}
