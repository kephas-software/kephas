// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDiscardChangesCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for discard changes commands.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IDiscardChangesCommand<in TDataContext> : IDataCommand<IDataOperationContext, IDataCommandResult>
        where TDataContext : IDataContext
    {
    }
}