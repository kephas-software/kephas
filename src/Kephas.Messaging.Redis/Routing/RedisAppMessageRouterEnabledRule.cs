// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisAppMessageRouterEnabledRule.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Redis.Routing
{
    using System;

    using Kephas.Behaviors;
    using Kephas.Interaction;
    using Kephas.Messaging.Distributed.Routing;
    using Kephas.Redis;
    using Kephas.Redis.Interaction;
    using Kephas.Services.Behaviors;

    /// <summary>
    /// Rule for enabling/disabling the Redis app message router.
    /// </summary>
    public class RedisAppMessageRouterEnabledRule
        : EnabledServiceBehaviorRuleBase<IMessageRouter, RedisAppMessageRouter>, IDisposable
    {
        private readonly IEventSubscription? redisClientStartedSubscription;
        private readonly IEventSubscription? redisClientStoppingSubscription;

        private bool isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisAppMessageRouterEnabledRule"/> class.
        /// </summary>
        /// <param name="redisConnectionManager">The Redis connection manager.</param>
        /// <param name="eventHub">The event hub.</param>
        public RedisAppMessageRouterEnabledRule(
            IRedisConnectionManager redisConnectionManager,
            IEventHub eventHub)
        {
            this.isEnabled = redisConnectionManager.IsInitialized;
            this.redisClientStartedSubscription = eventHub.Subscribe<ConnectionManagerStartedSignal>((e, ctx) => this.isEnabled = true);
            this.redisClientStoppingSubscription = eventHub.Subscribe<ConnectionManagerStoppingSignal>((e, ctx) => this.isEnabled = false);
        }

        /// <summary>
        /// Returns <see cref="BehaviorValue.True"/> if the Redis connection is initialized, <see cref="BehaviorValue.False"/> otherwise.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// <see cref="BehaviorValue.True"/> if the Redis connection is initialized, <see cref="BehaviorValue.False"/> otherwise.
        /// </returns>
        public override IBehaviorValue<bool> GetValue(IServiceBehaviorContext<IMessageRouter> context)
        {
            return this.isEnabled ? BehaviorValue.True : BehaviorValue.False;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.redisClientStartedSubscription?.Dispose();
            this.redisClientStoppingSubscription?.Dispose();
        }
    }
}