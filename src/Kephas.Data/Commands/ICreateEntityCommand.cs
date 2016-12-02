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
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface ICreateEntityCommand<in TDataContext, TEntity> : IDataCommand<ICreateEntityContext, ICreateEntityResult<TEntity>>
        where TDataContext : IDataContext
        where TEntity : class
    {
    }
}