﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipesMessagingTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis messaging test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Pipes.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas;
    using Kephas.Application;
    using Kephas.Commands;
    using Kephas.Configuration;
    using Kephas.Cryptography;
    using Kephas.Logging;
    using Kephas.Messaging.Distributed;
    using Kephas.Messaging.Pipes.Routing;
    using Kephas.Orchestration;
    using Kephas.Serialization.Json;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using Kephas.Testing.Services;

    public abstract class PipesMessagingTestBase : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(IAppLifecycleBehavior).Assembly,         // Kephas.Application.Abstractions
                typeof(IAppManager).Assembly,                   // Kephas.Application
                typeof(IConfiguration<>).Assembly,              // Kephas.Configuration
                typeof(IEncryptionService).Assembly,            // Kephas.Security
                typeof(IMessageBroker).Assembly,                // Kephas.Messaging.Distributed
                typeof(IMessageProcessor).Assembly,             // Kephas.Messaging
                typeof(PipesAppMessageRouter).Assembly,         // Kephas.Messaging.Pipes
                typeof(IOrchestrationManager).Assembly,         // Kephas.Orchestration
                typeof(ICommandProcessor).Assembly,             // Kephas.Commands
                typeof(JsonSerializer).Assembly,                // Kephas.Serialization.NewtonsoftJson
            };
        }

        protected override IAppServiceCollectionBuilder CreateServicesBuilder(
            IAppServiceCollection? appServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var builder = base.CreateServicesBuilder(appServices, logManager, appRuntime);
            appServices = builder.AppServices;
            if (!appServices.Contains(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(appServices));
                appServices.Add<IAppContext>(() => lazyAppContext.Value);
            }

            return builder;
        }

        protected IServiceProvider BuildServiceProvider()
        {
            return this.CreateServicesBuilder().BuildWithDependencyInjection();
        }

        protected IServiceProvider BuildServiceProvider(Action<IAppServiceCollectionBuilder> servicesBuilderConfig)
        {
            var builder = this.CreateServicesBuilder(this.CreateAppServices());
            servicesBuilderConfig(builder);
            return builder.BuildWithDependencyInjection();
        }
    }
}
