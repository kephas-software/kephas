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
    using Kephas.Data.Commands.Composition;
    using Kephas.Services;

    /// <summary>
    /// Contract for persist changes commands.
    /// </summary>
    [AppServiceContract(AsOpenGeneric = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IPersistChangesCommand : IDataCommand<IPersistChangesContext, IDataCommandResult>
    {
    }
}