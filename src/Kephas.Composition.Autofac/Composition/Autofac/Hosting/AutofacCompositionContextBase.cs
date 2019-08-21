// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac composition context base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using global::Autofac;

    using Kephas.Composition.Autofac.Resources;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An Autofac composition context base.
    /// </summary>
    public abstract class AutofacCompositionContextBase : ICompositionContext
    {
        private static ConcurrentDictionary<ILifetimeScope, ICompositionContext> map = new ConcurrentDictionary<ILifetimeScope, ICompositionContext>();

        private ILifetimeScope innerContainer;

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />.
        /// </returns>
        public object GetExport(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            return this.innerContainer.Resolve(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="contractType" />.
        /// </returns>
        public IEnumerable<object> GetExports(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            var collectionContract = typeof(IEnumerable<>).MakeGenericType(contractType);
            return (IEnumerable<object>)this.innerContainer.Resolve(collectionContract);
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T GetExport<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            return this.innerContainer.Resolve<T>();
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public IEnumerable<T> GetExports<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            return this.innerContainer.Resolve<IEnumerable<T>>();
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object TryGetExport(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            if (this.innerContainer.TryResolve(contractType, out var service))
            {
                return service;
            }

            return null;
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">Optional. The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T TryGetExport<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            if (this.innerContainer.TryResolve<T>(out var service))
            {
                return service;
            }

            return default;
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <param name="scopeName">Optional. The scope name. If not provided the
        ///                         <see cref="F:Kephas.Composition.CompositionScopeNames.Default" />
        ///                         scope name is used.</param>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public ICompositionContext CreateScopedContext(string scopeName = CompositionScopeNames.Default)
        {
            var scopedContext = this.innerContainer.BeginLifetimeScope(scopeName);
            return GetOrAddCompositionContext(scopedContext);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Tries to get the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="container">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The composition context wrapper.
        /// </returns>
        internal static ICompositionContext TryGetCompositionContext(ILifetimeScope container, bool createNewIfMissing)
        {
            if (map.TryGetValue(container, out var compositionContext))
            {
                return compositionContext;
            }

            return createNewIfMissing ? new AutofacScopedCompositionContext(container) : null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.innerContainer == null)
            {
                return;
            }

            map.TryRemove(this.innerContainer, out _);
            this.innerContainer.Dispose();
            this.innerContainer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContextBase"/> class.
        /// </summary>
        /// <param name="container">The inner container.</param>
        protected void Initialize(ILifetimeScope container)
        {
            Requires.NotNull(container, nameof(container));

            this.innerContainer = container;
            map.TryAdd(container, this);
        }

        /// <summary>
        /// Asserts that the container is not disposed.
        /// </summary>
        protected void AssertNotDisposed()
        {
            if (this.innerContainer == null)
            {
                throw new ObjectDisposedException(Strings.AutofacCompositionContainer_Disposed_Exception);
            }
        }

        /// <summary>
        /// Gets the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="scopedContextExport">The scoped context export.</param>
        /// <returns>
        /// The composition context.
        /// </returns>
        private static ICompositionContext GetOrAddCompositionContext(ILifetimeScope scopedContextExport)
        {
            return map.GetOrAdd(scopedContextExport, _ => new AutofacScopedCompositionContext(scopedContextExport));
        }
    }
}