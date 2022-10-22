// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Autofac
{
    using System.Collections.Concurrent;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Logging;

    /// <summary>
    /// An Autofac injection container.
    /// </summary>
    public class AutofacServiceProvider : AutofacServiceProviderBase, IServiceProviderContainer
    {
        private readonly ConcurrentDictionary<IComponentContext, IServiceProvider> map;

        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceProvider"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="logger">The log manager.</param>
        public AutofacServiceProvider(ContainerBuilder containerBuilder, ILogger? logger)
            : base(null, logger)
        {
            this.map = new ConcurrentDictionary<IComponentContext, IServiceProvider>();

            var registration = RegistrationBuilder
                .ForDelegate((c, p) => this.TryGetServiceProvider(c, createNewIfMissing: true)!)
                .As<IServiceProvider>()
                .InstancePerLifetimeScope()
                .CreateRegistration();
            containerBuilder.RegisterComponent(registration);

            this.container = containerBuilder.Build();
            this.Initialize(this.container);
            this.map.TryAdd(this.container, this);
        }

        /// <summary>
        /// Tries to get the injection wrapper for the provided component context.
        /// </summary>
        /// <param name="context">The component context.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The injection wrapper.
        /// </returns>
        public IServiceProvider? TryGetServiceProvider(IComponentContext context, bool createNewIfMissing)
        {
            var lifetimeScope = context.GetLifetimeScope();
            var key = "root".Equals(lifetimeScope.Tag) ? this.container : lifetimeScope;
            if (this.map.TryGetValue(key, out var injector))
            {
                return injector;
            }

            if (!createNewIfMissing)
            {
                return null;
            }

            return this.GetServiceProvider(lifetimeScope);
        }

        /// <summary>
        /// Cleanups the given injector.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        public void HandleDispose(ILifetimeScope lifetimeScope)
        {
            this.map.TryRemove(lifetimeScope, out _);
        }

        /// <summary>
        /// Gets the injector wrapper for the provided lifetime scope.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <returns>
        /// The injector.
        /// </returns>
        public IServiceProvider GetServiceProvider(ILifetimeScope scope)
        {
            return this.map.GetOrAdd(scope, _ => new AutofacScopedServiceProvider(this, scope, this.Logger));
        }
    }
}