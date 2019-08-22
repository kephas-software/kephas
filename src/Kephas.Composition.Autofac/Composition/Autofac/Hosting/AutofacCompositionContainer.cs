// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac composition container class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using System.Collections.Concurrent;

    using global::Autofac;
    using global::Autofac.Builder;

    /// <summary>
    /// An Autofac composition container.
    /// </summary>
    public class AutofacCompositionContainer : AutofacCompositionContextBase, ICompositionContainer
    {
        private readonly ConcurrentDictionary<IComponentContext, ICompositionContext> map;

        private IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContainer"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacCompositionContainer(ContainerBuilder containerBuilder)
            : base(null)
        {
            this.map = new ConcurrentDictionary<IComponentContext, ICompositionContext>();

            var registration = RegistrationBuilder
                .ForDelegate((c, p) => this.TryGetCompositionContext(c, createNewIfMissing: true))
                .As<ICompositionContext>()
                .InstancePerLifetimeScope()
                .CreateRegistration();
            containerBuilder.RegisterComponent(registration);

            this.container = containerBuilder.Build();
            this.Initialize(this.container);
            this.map.TryAdd(this.container, this);
        }

        /// <summary>
        /// Tries to get the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="context">The component context.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The composition context wrapper.
        /// </returns>
        public ICompositionContext TryGetCompositionContext(IComponentContext context, bool createNewIfMissing)
        {
            var lifetimeScope = context.GetLifetimeScope();
            var key = "root".Equals(lifetimeScope.Tag) ? this.container : lifetimeScope;
            if (this.map.TryGetValue(key, out var compositionContext))
            {
                return compositionContext;
            }

            if (!createNewIfMissing)
            {
                return null;
            }

            return this.GetCompositionContext(lifetimeScope);
        }

        /// <summary>
        /// Cleanups the given composition context.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        public void HandleDispose(ILifetimeScope lifetimeScope)
        {
            this.map.TryRemove(lifetimeScope, out _);
        }

        /// <summary>
        /// Gets the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <returns>
        /// The composition context.
        /// </returns>
        public ICompositionContext GetCompositionContext(ILifetimeScope scope)
        {
            return this.map.GetOrAdd(scope, _ => new AutofacScopedCompositionContext(this, scope));
        }
    }
}