// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IFindCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Data.Commands.Composition;
    using Kephas.Services;

    /// <summary>
    /// Contract for find commands.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [AppServiceContract(AsOpenGeneric = true, MetadataAttributes = new[] { typeof(DataContextTypeAttribute) })]
    public interface IFindCommand<TEntity> : IDataCommand<IFindContext<TEntity>, IFindResult<TEntity>>
        where TEntity : class
    {
    }
}