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
    using global::Autofac.Core.Resolving;

    using Kephas.Composition.Autofac.Resources;

    /// <summary>
    /// An Autofac composition container.
    /// </summary>
    public class AutofacCompositionContainer : AutofacCompositionContextBase, ICompositionContainer
    {
        private readonly ConcurrentDictionary<IComponentContext, ICompositionContext> map;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContainer"/> class.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        public AutofacCompositionContainer(ContainerBuilder containerBuilder)
            : base(null)
        {
            this.map = new ConcurrentDictionary<IComponentContext, ICompositionContext>();

            containerBuilder.Register((c, p) => this.TryGetCompositionContext(c, createNewIfMissing: true))
                .As<ICompositionContext>()
                .InstancePerLifetimeScope();

            var container = containerBuilder.Build();
            this.Initialize(container);
            this.map.TryAdd(container, this);
        }

        /// <summary>
        /// Tries to get the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="container">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The composition context wrapper.
        /// </returns>
        public ICompositionContext TryGetCompositionContext(IComponentContext container, bool createNewIfMissing)
        {
            if (this.map.TryGetValue(container, out var compositionContext))
            {
                return compositionContext;
            }

            if (!createNewIfMissing)
            {
                return null;
            }

            if (container is ILifetimeScope lifetimeScope)
            {
                return this.GetCompositionContext(lifetimeScope);
            }

            if (container is IInstanceLookup instanceLookup)
            {
                return this.GetCompositionContext(instanceLookup.ActivationScope);
            }

            throw new CompositionException(Strings.AutofacCompositionContainer_MismatchedLifetimeScope_Exception);
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