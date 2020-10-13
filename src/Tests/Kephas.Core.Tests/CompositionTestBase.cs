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
    using Kephas.Composition;
    using Kephas.Composition.Hosting;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class CompositionTestBase : TestBase
    {
        public virtual LiteCompositionContainerBuilder WithContainerBuilder(IAmbientServices ambientServices = null, ILogManager? logManager = null, IAppRuntime appRuntime = null)
        {
            logManager = logManager ?? new NullLogManager();
            appRuntime = appRuntime ?? this.CreateDefaultAppRuntime(logManager);

            ambientServices = ambientServices ?? new AmbientServices();
            ambientServices
                .Register(logManager)
                .WithAppRuntime(appRuntime);
            return new LiteCompositionContainerBuilder(new CompositionRegistrationContext(ambientServices));
        }

        public ICompositionContext CreateContainer(params Assembly[] assemblies)
        {
            return CreateContainer(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        public virtual ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<LiteCompositionContainerBuilder> config = null)
        {
            ambientServices = ambientServices ?? new AmbientServices();
            var containerBuilder = WithContainerBuilder(ambientServices)
                    .WithAssemblies(GetDefaultConventionAssemblies())
                    .WithAssemblies(assemblies ?? new Assembly[0])
                    .WithParts(parts ?? new Type[0]);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.CreateContainer();
            ambientServices.Register(container);
            return container;
        }

        public ICompositionContext CreateContainerWithBuilder(Action<LiteCompositionContainerBuilder> config = null)
        {
            var builder = WithContainerBuilder()
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly);
            config?.Invoke(builder);
            return builder.CreateContainer();
        }

        public ICompositionContext CreateContainerWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return WithContainerBuilder(ambientServices)
                .WithAssembly(typeof(ICompositionContext).GetTypeInfo().Assembly)
                .WithParts(types)
                .CreateContainer();
        }

        public virtual IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(ICompositionContext).GetTypeInfo().Assembly,     /* Kephas.Core*/
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}