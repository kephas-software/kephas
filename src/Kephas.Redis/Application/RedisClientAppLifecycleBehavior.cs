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
    using Kephas.Services;
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
        /// <param name="connectionFactories">The connection factories.</param>
        public RedisClientAppLifecycleBehavior(ILazyEnumerable<IConnectionFactory, AppServiceMetadata> connectionFactories)
        {
            this.redisConnectionFactory = connectionFactories.TryGetService(m => m.ServiceName == RedisConnectionFactory.ConnectionKind);
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
        {
            if (this.redisConnectionFactory != null)
            {
                await ServiceHelper.InitializeAsync(this.redisConnectionFactory, cancellationToken: cancellationToken).PreserveThreadContext();
            }

            return true.ToOperationResult();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task<IOperationResult> AfterAppFinalizeAsync(CancellationToken cancellationToken = default)
        {
            if (this.redisConnectionFactory != null)
            {
                await ServiceHelper.FinalizeAsync(this.redisConnectionFactory, cancellationToken: cancellationToken).PreserveThreadContext();
            }

            return true.ToOperationResult();
        }
    }
}
