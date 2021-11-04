// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeElementInfoFactoryAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Runtime.Factories;
    using Kephas.Services;

    /// <summary>
    /// Application lifecycle behavior for registering <see cref="IRuntimeTypeInfoFactory"/> services.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class RuntimeElementInfoFactoryAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly ICollection<Lazy<IRuntimeElementInfoFactory>> lazyFactories;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeElementInfoFactoryAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="lazyFactories">The lazy factories.</param>
        public RuntimeElementInfoFactoryAppLifecycleBehavior(IRuntimeTypeRegistry typeRegistry, ICollection<Lazy<IRuntimeElementInfoFactory>> lazyFactories)
        {
            this.typeRegistry = typeRegistry;
            this.lazyFactories = lazyFactories;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <remarks>
        /// To interrupt the application initialization, simply throw an appropriate exception.
        /// </remarks>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result yielding the operation result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            foreach (var lazyFactory in this.lazyFactories)
            {
                this.typeRegistry.RegisterFactory(lazyFactory.Value);
            }

            return Task.FromResult((IOperationResult)true.ToOperationResult());
        }
    }
}