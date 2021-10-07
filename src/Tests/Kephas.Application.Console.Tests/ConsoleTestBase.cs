// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Commands;
    using Kephas.Injection;
    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Testing.Injection;

    public abstract class ConsoleTestBase : InjectionTestBase
    {
        public override IInjector CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            ambientServices ??= this.CreateAmbientServices();
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
                            typeof(IAppManager).Assembly,           // Kephas.Application
                            typeof(ICommandProcessor).Assembly,     // Kephas.Commands
                            typeof(IConsole).Assembly,              // Kephas.Application.Console
                        });

            return assemblies;
        }
    }
}
