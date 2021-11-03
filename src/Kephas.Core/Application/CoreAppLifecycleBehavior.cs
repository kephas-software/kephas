// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Configuration.Runtime;
    using Kephas.Operations;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Application lifecycle behavior for the core.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class CoreAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="typeRegistry">The runtime type registry.</param>
        public CoreAppLifecycleBehavior(IRuntimeTypeRegistry typeRegistry)
        {
            this.typeRegistry = typeRegistry;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            this.typeRegistry.RegisterFactory(new ConfigurationTypeInfoFactory());

            return Task.FromResult<IOperationResult>(true.ToOperationResult());
        }
    }
}