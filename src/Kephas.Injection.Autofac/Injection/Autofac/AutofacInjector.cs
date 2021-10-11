// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac
{
    using System.Collections.Concurrent;

    using global::Autofac;
    using global::Autofac.Builder;
    using Kephas.Injection;

    /// <summary>
    /// An Autofac injection container.
    /// </summary>
    public class AutofacInjector : AutofacInjectorBase, IInjectionContainer
    {
        private readonly ConcurrentDictionary<IComponentContext, IInjector> map;

        private readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInjector"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacInjector(ContainerBuilder containerBuilder)
            : base(null)
        {
            this.map = new ConcurrentDictionary<IComponentContext, IInjector>();

            var registration = RegistrationBuilder
                .ForDelegate((c, p) => this.TryGetInjector(c, createNewIfMissing: true)!)
                .As<IInjector>()
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
        public IInjector? TryGetInjector(IComponentContext context, bool createNewIfMissing)
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

            return this.GetInjector(lifetimeScope);
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
        public IInjector GetInjector(ILifetimeScope scope)
        {
            return this.map.GetOrAdd(scope, _ => new AutofacScopedInjector(this, scope));
        }
    }
}