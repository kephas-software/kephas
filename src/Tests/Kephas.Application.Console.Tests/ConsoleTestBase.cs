// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;
using Kephas.Injection.Lite.Hosting;

namespace Kephas.Application.Console.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Commands;
    using Kephas.Logging;
    using Kephas.Testing.Composition;

    public abstract class ConsoleTestBase : InjectionTestBase
    {
        public override IInjector CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= new AmbientServices();
            if (!ambientServices.IsRegistered(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(ambientServices));
                ambientServices.Register<IAppContext>(() => lazyAppContext.Value);
            }

            return base.CreateContainer(ambientServices, assemblies, parts, config);
        }

        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = base.GetDefaultConventionAssemblies().ToList();
            assemblies.AddRange(new[]
                        {
                            typeof(IAppManager).Assembly,           // Kephas.Application
                            typeof(ICommandProcessor).Assembly,     // Kephas.Commands
                            typeof(IConsole).Assembly,              // Kephas.Application.Console
                        });

            return assemblies;
        }
    }
}
