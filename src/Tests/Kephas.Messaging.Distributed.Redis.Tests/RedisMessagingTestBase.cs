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
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Connectivity;
    using Kephas.Cryptography;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Redis.Routing;
    using Kephas.Redis;
    using Kephas.Redis.Connectivity;
    using Kephas.Serialization.Json;
    using Kephas.Testing.Injection;

    public abstract class RedisMessagingTestBase : InjectionTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IAppLifecycleBehavior).Assembly,         // Kephas.Application.Abstractions
                typeof(IAppManager).Assembly,                   // Kephas.Application
                typeof(IConnectionFactory).Assembly,            // Kephas.Connectivity
                typeof(IConfiguration<>).Assembly,              // Kephas.Configuration
                typeof(IEncryptionService).Assembly,            // Kephas.Security
                typeof(IMessageBroker).Assembly,                // Kephas.Messaging.Distributed
                typeof(IMessageProcessor).Assembly,             // Kephas.Messaging
                typeof(RedisConnectionFactory).Assembly,        // Kephas.Redis
                typeof(RedisAppMessageRouter).Assembly,         // Kephas.Messaging.Distributed.Redis
                typeof(JsonSerializer).Assembly,                // Kephas.Serialization.NewtonsoftJson
            };
        }

        public override IServiceProvider BuildServiceProvider(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= this.CreateAmbientServices();
            if (!ambientServices.Contains(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Add<IAppContext>(() => lazyAppContext.Value);
            }

            return base.BuildServiceProvider(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }
    }
}
