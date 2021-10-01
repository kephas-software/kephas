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
    using Kephas.Injection;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Testing.Application;
    using NSubstitute;

    public abstract class OrchestrationTestBase : ApplicationTestBase
    {
        public override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
                                {
                                    typeof(IMessageBroker).Assembly,
                                    typeof(IMessageProcessor).Assembly,
                                    typeof(IOrchestrationManager).Assembly,
                                    typeof(ICommandProcessor).Assembly,
                                };
            return assemblies;
        }

        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var oldConfig = config;
            config = b =>
            {
                var appContext = Substitute.For<IAppContext>();
                b.WithFactory(() => appContext);
                oldConfig?.Invoke(b);
            };
            return base.CreateInjector(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }
    }
}
