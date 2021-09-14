// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the orchestration test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Application;
using Kephas.Composition;
using Kephas.Composition.Lite.Hosting;
using Kephas.Logging;
using NSubstitute;

namespace Kephas.Tests.Orchestration
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Commands;
    using Kephas.Messaging;
    using Kephas.Messaging.Distributed;
    using Kephas.Orchestration;
    using Kephas.Testing.Application;

    public abstract class OrchestrationTestBase : ApplicationTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
                                {
                                    typeof(IMessageBroker).Assembly,
                                    typeof(IMessageProcessor).Assembly,
                                    typeof(IOrchestrationManager).Assembly,
                                    typeof(ICommandProcessor).Assembly,
                                };
            return assemblies;
        }

        public override ICompositionContext CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteCompositionContainerBuilder>? config = null,
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
            return base.CreateContainer(ambientServices, assemblies, parts, config, logManager, appRuntime);
        }
    }
}
