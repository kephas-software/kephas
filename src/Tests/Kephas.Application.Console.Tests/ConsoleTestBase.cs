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
    using Kephas.Logging;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using Kephas.Testing.Services;

    public abstract class ConsoleTestBase : TestBase
    {
        /// <summary>
        /// Creates a <see cref="IAppServiceCollectionBuilder"/> for further configuration.
        /// </summary>
        /// <param name="ambientServices">Optional. The ambient services. If not provided, a new instance
        ///                               will be created as linked to the newly created container.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        /// <param name="appRuntime">Optional. The application runtime.</param>
        /// <returns>
        /// A LiteInjectorBuilder.
        /// </returns>
        protected override IAppServiceCollectionBuilder CreateServicesBuilder(
            IAmbientServices? ambientServices = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var builder = base.CreateServicesBuilder(ambientServices, logManager, appRuntime);
            ambientServices = builder.AmbientServices;
            if (!ambientServices.Contains(typeof(IAppContext)))
            {
                var lazyAppContext = new Lazy<IAppContext>(() => new Kephas.Application.AppContext(builder));
                ambientServices.Add<IAppContext>(() => lazyAppContext.Value);
            }

            return builder;
        }

        /// <summary>
        /// Gets the default convention types to be considered when building the container. By default it includes Kephas.Core.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention types in
        /// this collection.
        /// </returns>
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = base.GetAssemblies().ToList();
            assemblies.AddRange(new[]
                        {
                            typeof(IAppContext).Assembly,           // Kephas.Application.Abstractions
                            typeof(IAppManager).Assembly,           // Kephas.Application
                            typeof(ICommandProcessor).Assembly,     // Kephas.Commands
                            typeof(IConsole).Assembly,              // Kephas.Application.Console
                        });

            return assemblies;
        }
    }
}
