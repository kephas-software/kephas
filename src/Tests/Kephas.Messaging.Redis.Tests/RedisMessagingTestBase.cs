// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisMessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Redis.Tests
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Redis;
    using Kephas.Serialization.Json;
    using Kephas.Testing.Composition;

    public abstract class RedisMessagingTestBase : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(IAppManager).Assembly,                   // Kephas.Application
                typeof(IMessageProcessor).Assembly,             // Kephas.Messaging
                typeof(IRedisConnectionFactory).Assembly,                  // Kephas.Redis
                typeof(RedisAppMessageRouter).Assembly,         // Kephas.Messaging.Redis
                typeof(JsonSerializer).Assembly,                // Kephas.Serialization.Josn
            };
        }
    }
}
