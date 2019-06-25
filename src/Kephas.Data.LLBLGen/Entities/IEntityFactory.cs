// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using Kephas.Services;

    /// <summary>
    /// Base contract for entity factories.
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// Creates a new entity instance.
        /// </summary>
        /// <returns>
        /// New entity instance.
        /// </returns>
        object Create();
    }

    /// <summary>
    /// Application service contract for entity factories.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [AppServiceContract(ContractType = typeof(IEntityFactory), AllowMultiple = true)]
    public interface IEntityFactory<TEntity> : IEntityFactory
    {
    }
}