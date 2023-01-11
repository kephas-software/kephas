// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Services.Builder;
    using Kephas.Testing;

    public abstract class CommandsTestBase : TestBase
    {
        protected IServiceProvider BuildServiceProvider()
        {
            return this.CreateServicesBuilder().BuildWithDependencyInjection();
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

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = base.GetAssemblies().ToList();
            assemblies.AddRange(new[]
                        {
                            typeof(IAppManager).Assembly,               // Kephas.Application
                            typeof(IMessageProcessor).Assembly,         // Kephas.Messaging
                            typeof(ICommandProcessor).Assembly,         // Kephas.Commands
                            typeof(MessagingCommandRegistry).Assembly,  // Kephas.Commands.Messaging
                        });

            return assemblies;
        }
    }
}
