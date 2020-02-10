// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the transaction base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Transactions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Operations;

    /// <summary>
    /// A transaction base.
    /// </summary>
    public abstract class TransactionBase : ITransaction
    {
        private readonly PluginData pluginData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionBase"/> class.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        public TransactionBase(PluginData pluginData)
        {
            Requires.NotNull(pluginData, nameof(pluginData));

            this.pluginData = pluginData;
        }

        /// <summary>
        /// Undoes the operations in the given context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the <see cref="IOperationResult"/>.
        /// </returns>
        public Task<IOperationResult> RollbackAsync(IPluginContext context, CancellationToken cancellationToken = default)
        {
            var undoCommands = this.GetUndoCommands(this.pluginData);

            var result = new OperationResult();
            var opResult = Profiler.WithStopwatch(() =>
            {
                foreach (var (key, undoCommand) in undoCommands)
                {
                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        undoCommand.Execute(context);
                        this.pluginData.Data.Remove(key);
                    }
                    catch (Exception ex)
                    {
                        result.MergeException(ex);
                    }
                }
            });

            return Task.FromResult<IOperationResult>(result.Complete(opResult.Elapsed));
        }

        /// <summary>
        /// Adds an undo command to the transaction log.
        /// </summary>
        /// <param name="undoCommand">The undo command.</param>
        /// <returns>
        /// This transaction.
        /// </returns>
        public ITransaction AddCommand(UndoCommandBase undoCommand)
        {
            Requires.NotNull(undoCommand, nameof(undoCommand));

            var cmdKeyPart = this.GetCommandKeyPart();
            this.pluginData.Data.Add($"{cmdKeyPart}{undoCommand.Index}", undoCommand.ToString());

            return this;
        }

        /// <summary>
        /// Gets the command key part.
        /// </summary>
        /// <returns>
        /// The command key part.
        /// </returns>
        protected virtual string GetCommandKeyPart() => UndoCommandBase.KeyPart;

        /// <summary>
        /// Gets the undo commands.
        /// </summary>
        /// <param name="pluginData">Information describing the plugin.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the undo commands in this collection.
        /// </returns>
        protected virtual IEnumerable<(string key, UndoCommandBase command)> GetUndoCommands(PluginData pluginData)
        {
            var cmdKeyPart = this.GetCommandKeyPart();
            var cmds = pluginData.Data.Keys
                .Where(k => k.StartsWith(cmdKeyPart))
                .Select(k =>
                {
                    var cmd = UndoCommandBase.Parse(pluginData.Data[k]);
                    cmd.Index = int.Parse(k.Substring(cmdKeyPart.Length));
                    return (key: k, command: cmd);
                })
                .OrderByDescending(cmd => cmd.command.Index)
                .ToList();
            return cmds;
        }
    }
}
