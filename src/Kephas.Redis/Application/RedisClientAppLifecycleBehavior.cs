// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisClientAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis client app lifecycle behavior class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Connectivity;
    using Kephas.Injection;
    using Kephas.Operations;
    using Kephas.Redis.Connectivity;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The Redis client application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class RedisClientAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IConnectionFactory? redisConnectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisClientAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public RedisClientAppLifecycleBehavior(IInjector injector)
        {
            this.redisConnectionFactory = injector.TryResolve<IConnectionFactory>(RedisConnectionFactory.ConnectionKind);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<IOperationResult> BeforeAppInitializeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            if (this.redisConnectionFactory != null)
            {
                await ServiceHelper.InitializeAsync(this.redisConnectionFactory, appContext, cancellationToken).PreserveThreadContext();
            }

            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> AfterAppFinalizeAsync(
            IAppContext appContext,
            CancellationToken cancellationToken = default)
        {
            if (this.redisConnectionFactory != null)
            {
                await ServiceHelper.FinalizeAsync(this.redisConnectionFactory, appContext, cancellationToken).PreserveThreadContext();
            }

            return true.ToOperationResult();
        }
    }
}
