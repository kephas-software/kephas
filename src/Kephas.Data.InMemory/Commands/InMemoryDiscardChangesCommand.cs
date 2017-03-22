// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory discard changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Discard changes command for <see cref="InMemoryDataContext"/>.
    /// </summary>
    public class InMemoryDiscardChangesCommand : DiscardChangesCommandBase<InMemoryDataContext>
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        public override void Execute(IDataOperationContext operationContext)
        {
        }
    }
}