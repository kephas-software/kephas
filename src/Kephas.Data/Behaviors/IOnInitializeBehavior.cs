// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnInitializeBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IOnInitializeBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Contract for the behavior invoked upon entity intialization.
    /// </summary>
    public interface IOnInitializeBehavior
    {
        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entitiy to be initialized.</param>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAsync(object entity, IEntityInfo entityInfo, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}