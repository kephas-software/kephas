// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrderedLazyServiceCollection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrderedLazyServiceCollection interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Interface for ordered lazy service collection.
    /// </summary>
    /// <typeparam name="TTargetContract">Type of the target service contract.</typeparam>
    /// <typeparam name="TMetadata">Type of the service metadata.</typeparam>
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IOrderedLazyServiceCollection<TTargetContract, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TMetadata> : IEnumerable<Lazy<TTargetContract, TMetadata>>
        where TMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Gets the service factories in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered service factories.
        /// </returns>
        IEnumerable<Lazy<TTargetContract, TMetadata>> GetServiceFactories(
            Func<Lazy<TTargetContract, TMetadata>, bool>? filter = null);

        /// <summary>
        /// Gets the services in the appropriate order.
        /// </summary>
        /// <param name="filter">Optional. Specifies a filter.</param>
        /// <returns>
        /// The ordered services.
        /// </returns>
        IEnumerable<TTargetContract> GetServices(
            Func<Lazy<TTargetContract, TMetadata>, bool>? filter = null);
    }
}