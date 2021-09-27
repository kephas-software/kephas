// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBulkUpdateCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBulkUpdateCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for commands updating one or more entities based on a predicate, directly in the database.
    /// </summary>
    /// <remarks>
    /// The operation skips all data behaviors and sends the commands directly to the database.
    /// </remarks>
    [AppServiceContract(AllowMultiple = true)]
    public interface IBulkUpdateCommand : IDataCommand<IBulkUpdateContext, IBulkDataOperationResult>
    {
    }
}