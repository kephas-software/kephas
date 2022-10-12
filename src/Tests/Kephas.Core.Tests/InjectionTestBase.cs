// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionTestBase.cs" company="Kephas Software SRL">
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
    using Kephas.Injection.Builder;
    using Kephas.Injection.Lite.Builder;
    using Kephas.Logging;
    using Kephas.Services;
    using Kephas.Testing;
    using Kephas.Testing.Injection;

    /// <summary>
    /// Base class for tests using composition.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class InjectionTestBase : TestBase
    {
        public virtual LiteInjectorBuilder WithInjectorBuilder(IAmbientServices? ambientServices = null, ILogManager? logManager = null, IAppRuntime? appRuntime = null)
        {
            logManager ??= new NullLogManager();
            appRuntime ??= this.CreateDefaultAppRuntime(logManager);

            ambientServices ??= this.CreateAmbientServices();
            ambientServices
                .Add(logManager)
                .WithAppRuntime(appRuntime);
            return new LiteInjectorBuilder(new AppServiceCollectionBuilder(ambientServices));
        }

        public IServiceProvider CreateInjector(params Assembly[] assemblies)
        {
            return this.CreateInjector(assemblies: (IEnumerable<Assembly>)assemblies);
        }

        public virtual IServiceProvider CreateInjector(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<LiteInjectorBuilder>? config = null)
        {
            ambientServices ??= this.CreateAmbientServices();
            var containerBuilder = this.WithInjectorBuilder(ambientServices)
                    .WithAssemblies(this.GetAssemblies())
                    .WithAssemblies(assemblies ?? Array.Empty<Assembly>())
                    .WithParts(parts ?? Type.EmptyTypes);

            config?.Invoke(containerBuilder);

            var container = containerBuilder.Build();
            ambientServices.Add(container);
            return container;
        }

        public IServiceProvider CreateInjectorWithBuilder(Action<LiteInjectorBuilder>? config = null)
        {
            var builder = this.WithInjectorBuilder()
                .WithAssemblies(typeof(IServiceProvider).Assembly, typeof(IInjectableFactory).Assembly);
            config?.Invoke(builder);
            return builder.Build();
        }

        public IServiceProvider CreateInjectorWithBuilder(IAmbientServices ambientServices, params Type[] types)
        {
            return this.WithInjectorBuilder(ambientServices)
                .WithAssemblies(typeof(IServiceProvider).Assembly, typeof(IInjectableFactory).Assembly)
                .WithParts(types)
                .Build();
        }

        public virtual IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>
                       {
                           typeof(IServiceProvider).Assembly,          /* Kephas.Injection */
                       };
        }

        public virtual IEnumerable<Type> GetDefaultParts()
        {
            return new List<Type>();
        }
    }
}