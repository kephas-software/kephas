// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisMessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.Lite.Hosting;

namespace Kephas.Messaging.Redis.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Redis;
    using Kephas.Serialization.Json;
    using Kephas.Testing.Injection;

    public abstract class RedisMessagingTestBase : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(IAppManager).Assembly,                   // Kephas.Application
                typeof(IMessageBroker).Assembly,                // Kephas.Messaging.Distributed
                typeof(IMessageProcessor).Assembly,             // Kephas.Messaging
                typeof(IRedisConnectionManager).Assembly,       // Kephas.Redis
                typeof(RedisAppMessageRouter).Assembly,         // Kephas.Messaging.Redis
                typeof(JsonSerializer).Assembly,                // Kephas.Serialization.NewtonsoftJson
            };
        }

        public override IInjector CreateInjector(IAmbientServices? ambientServices = null, IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null, Action<LiteInjectorBuilder>? config = null, ILogManager? logManager = null, IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices();
            if (!ambientServices.IsRegistered(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Register<IAppContext>(() => lazyAppContext.Value);
            }

            return base.CreateInjector(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }
    }
}
