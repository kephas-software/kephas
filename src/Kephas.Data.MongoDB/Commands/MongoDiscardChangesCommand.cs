// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the mongo discard changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using Kephas.Data.Commands;

    /// <summary>
    /// Discard changes command for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoDiscardChangesCommand : DiscardChangesCommandBase
    {
        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="operationContext">Context for the operation.</param>
        public override void Execute(IDataOperationContext operationContext)
        {
        }
    }
}