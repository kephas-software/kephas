// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenBulkUpdateCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate update bulk command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;
    using Kephas.Logging;

    /// <summary>
    /// A LLBLGen bulk update command.
    /// </summary>
    [DataContextType(typeof(LLBLGenDataContext))]
    public class LLBLGenBulkUpdateCommand : DataCommandBase<IBulkUpdateContext, IBulkDataOperationResult>, IBulkUpdateCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenBulkUpdateCommand"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public LLBLGenBulkUpdateCommand(ILogManager logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="T:Kephas.Data.Commands.IDataCommandResult" />.
        /// </returns>
        public override Task<IBulkDataOperationResult> ExecuteAsync(IBulkUpdateContext operationContext, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}