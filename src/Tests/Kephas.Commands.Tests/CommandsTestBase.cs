// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the console test base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Testing.Injection;

    public abstract class CommandsTestBase  : TestBase
    {
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

            return base.BuildServiceProvider(ambientServices, assemblies, parts, config);
        }

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = base.GetAssemblies().ToList();
            assemblies.AddRange(new[]
                        {
                            typeof(IAppManager).Assembly,               // Kephas.Application
                            typeof(ICommandProcessor).Assembly,         // Kephas.Commands
                        });

            return assemblies;
        }
    }
}
