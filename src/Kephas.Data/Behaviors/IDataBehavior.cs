// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using Kephas.Services;

    /// <summary>
    /// Application service contract for data behaviors.
    /// </summary>
    /// <remarks>
    /// This contract is used only to collect the data behaviors for a certain type.
    /// The data behavior will then implement the <see cref="IDataBehavior{TEntity}"/>
    /// with the specific entity.
    /// </remarks>
    public interface IDataBehavior
    {
    }

    /// <summary>
    /// Generic application service contract for data behaviors.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    [SharedAppServiceContract(ContractType = typeof(IDataBehavior))]
    public interface IDataBehavior<TEntity> : IDataBehavior
    {
    }
}