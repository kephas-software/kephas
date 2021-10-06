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
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Testing.Injection;

    public abstract class CommandsTestBase : InjectionTestBase
    {
        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices();
            if (!ambientServices.IsRegistered(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Register<IAppContext>(() => lazyAppContext.Value);
            }

            return base.CreateInjector(ambientServices, assemblies, parts, config);
        }

        public override IEnumerable<Assembly> GetAssemblies()
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
