// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleTestBase.cs" company="Kephas Software SRL">
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
    using Kephas.Composition;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Testing.Composition;

    public abstract class CommandsTestBase : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IAmbientServices ambientServices = null, IEnumerable<Assembly> assemblies = null, IEnumerable<Type> parts = null, Action<LiteCompositionContainerBuilder> config = null)
        {
            ambientServices = ambientServices ?? new AmbientServices();
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
                            typeof(IAppManager).Assembly,               // Kephas.Application
                            typeof(ICommandProcessor).Assembly,         // Kephas.Commands
                        });

            return assemblies;
        }
    }
}
