// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class for tests using composition.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Injection;
    using Kephas.Injection.Hosting;
    using Kephas.Injection.Lite.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase : TestBase
    {
        public virtual LiteInjectorBuilder WithContainerBuilder(IAmbientServices? ambientServices = null, ILogManager? logManager = null, IAppRuntime? appRuntime = null)
        {
            logManager ??= new NullLogManager();
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices ??= new AmbientServices();
            ambientServices
                .Register(logManager)
                .WithAppRuntime(appRuntime);
            return new LiteInjectorBuilder(new InjectionRegistrationContext(ambientServices));
        }

        public IInjector CreateContainer(params Assembly[] assemblies)
        {
            return this.CreateContainer(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        public virtual IInjector CreateContainer(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteInjectorBuilder>? config = null)
        {
            ambientServices ??= new AmbientServices();
            var containerBuilder = this.WithContainerBuilder(ambientServices)
                    .WithAssemblies(this.GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? Array.Empty<Assembly>())
                    .WithParts(parts ?? Type.EmptyTypes);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateInjector();
            ambientServices.Register(container);
            return container;
        }

        public IInjector CreateContainerWithBuilder(Action<LiteInjectorBuilder>? config = null)
        {
            var builder = WithContainerBuilder()
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.CreateInjector();
        }

        public IInjector CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return this.WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(IInjector).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateInjector();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IInjector).GetTypeInfo().Assembly,     /* Kephas.Core*/
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}