// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IPersistChangesCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Contract for persist changes commands.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IPersistChangesCommand<in TDataContext> : IDataCommand<IPersistChangesContext, IDataCommandResult>
        where TDataContext : IDataContext
    {
    }
}