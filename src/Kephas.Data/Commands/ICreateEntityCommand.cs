// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICreateEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICreateEntityCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Services;

    /// <summary>
    /// Application service contract for commands creating an entity.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface ICreateEntityCommand : IDataCommand<ICreateEntityContext, ICreateEntityResult>
    {
    }
}