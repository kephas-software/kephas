// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Orchestration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Commands;
    using Kephas.Configuration;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Services.Builder;
    using Kephas.Testing.Application;
    using NSubstitute;

    public abstract class OrchestrationTestBase : ApplicationTestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(IConfiguration<>).Assembly,
                                    typeof(IMessageBroker).Assembly,
                                    typeof(IMessageProcessor).Assembly,
                                    typeof(IOrchestrationManager).Assembly,
                                    typeof(ICommandProcessor).Assembly,
                                };
            return assemblies;
        }

        /// <summary>
        /// Creates a <see cref="IAppServiceCollectionBuilder"/> for further configuration.
        /// </summary>
        /// <param name="appServices">Optional. The application services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A LiteInjectorBuilder.
        /// </returns>
        protected override IAppServiceCollectionBuilder CreateServicesBuilder(
            IAppServiceCollection? appServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var builder = base.CreateServicesBuilder(appServices, logManager, appRuntime);
            appServices = builder.AppServices;
            var appContext = Substitute.For<IAppContext>();
            appServices.Add<IAppContext>(_ => appContext);
            return builder;
        }
    }
}
