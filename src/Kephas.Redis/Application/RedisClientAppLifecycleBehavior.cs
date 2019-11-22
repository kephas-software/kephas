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
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The Redis client application lifecycle behavior.
    /// </summary>
    [ProcessingPriority(Priority.High)]
    public class RedisClientAppLifecycleBehavior : IAppLifecycleBehavior
    {
        private readonly IRedisConnectionFactory redisClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisClientAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="connectionFactory">The Redis connection factory.</param>
        public RedisClientAppLifecycleBehavior(IRedisConnectionFactory connectionFactory)
        {
            this.redisClient = connectionFactory;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public async Task BeforeAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            await ServiceHelper.InitializeAsync(this.redisClient, appContext, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task AfterAppInitializeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called before the application starts its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task BeforeAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous finalization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public async Task AfterAppFinalizeAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            await ServiceHelper.FinalizeAsync(this.redisClient, appContext, cancellationToken).PreserveThreadContext();
        }
    }
}
