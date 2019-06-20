﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenBulkDeleteCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate delete bulk command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;

    /// <summary>
    /// A LLBLGen bulk delete command.
    /// </summary>
    [DataContextType(typeof(LLBLGenDataContext))]
    public class LLBLGenBulkDeleteCommand : DataCommandBase<IBulkDeleteContext, IBulkDataOperationResult>, IBulkDeleteCommand
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="T:Kephas.Data.Commands.IDataCommandResult" />.
        /// </returns>
        public override Task<IBulkDataOperationResult> ExecuteAsync(IBulkDeleteContext operationContext, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}