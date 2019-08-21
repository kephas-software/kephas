// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF composition context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Hosting
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Composition;

    using Kephas.Composition.Mef.Resources;
    using Kephas.Composition.Mef.ScopeFactory;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A MEF composition context.
    /// </summary>
    public abstract class MefCompositionContextBase : ICompositionContext
    {
        private static ConcurrentDictionary<CompositionContext, ICompositionContext> map = new ConcurrentDictionary<CompositionContext, ICompositionContext>();

        private CompositionContext innerCompositionContext;

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An object implementing <paramref name="contractType"/>.</returns>
        public virtual object GetExport(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            var component = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.GetExport(contractType)
                              : this.innerCompositionContext.GetExport(contractType, contractName);
            return component;
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>An enumeration of objects implementing <paramref name="contractType"/>.</returns>
        public virtual IEnumerable<object> GetExports(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            var components = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.GetExports(contractType)
                              : this.innerCompositionContext.GetExports(contractType, contractName);
            return components;
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public virtual T GetExport<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            var component = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.GetExport<T>()
                              : this.innerCompositionContext.GetExport<T>(contractName);
            return component;
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public virtual IEnumerable<T> GetExports<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            var components = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.GetExports<T>()
                              : this.innerCompositionContext.GetExports<T>(contractName);
            return components;
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        public virtual object TryGetExport(Type contractType, string contractName = null)
        {
            this.AssertNotDisposed();

            object component;
            var successful = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.TryGetExport(contractType, out component)
                              : this.innerCompositionContext.TryGetExport(contractType, contractName, out component);
            return successful ? component : null;
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="contractName">The contract name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the provided contract was not found.
        /// </returns>
        public virtual T TryGetExport<T>(string contractName = null)
        {
            this.AssertNotDisposed();

            var successful = string.IsNullOrEmpty(contractName)
                              ? this.innerCompositionContext.TryGetExport(out T component)
                              : this.innerCompositionContext.TryGetExport(contractName, out component);
            return component;
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public virtual ICompositionContext CreateScopedContext()
        {
            var scopeProvider = this.GetExport<IMefScopeFactory>(CompositionScopeNames.Default);

            var scopedContextExport = scopeProvider.CreateScopedContextExport();
            return GetOrAddCompositionContext(scopedContextExport);
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
        /// <param name="context">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The composition context wrapper.
        /// </returns>
        internal static ICompositionContext TryGetCompositionContext(CompositionContext context, bool createNewIfMissing)
        {
            if (map.TryGetValue(context, out var compositionContext))
            {
                return compositionContext;
            }

            return createNewIfMissing ? new MefScopedCompositionContext(new Export<CompositionContext>(context, () => { })) : null;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.innerCompositionContext == null)
            {
                return;
            }

            map.TryRemove(this.innerCompositionContext, out _);
            var disposableInnerContainer = this.innerCompositionContext as IDisposable;
            disposableInnerContainer?.Dispose();

            this.innerCompositionContext = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MefCompositionContextBase"/> class.
        /// </summary>
        /// <param name="context">The inner container.</param>
        protected void Initialize(CompositionContext context)
        {
            Requires.NotNull(context, nameof(context));

            this.innerCompositionContext = context;
            map.TryAdd(context, this);
        }

        /// <summary>
        /// Asserts that the container is not disposed.
        /// </summary>
        protected void AssertNotDisposed()
        {
            if (this.innerCompositionContext == null)
            {
                throw new ObjectDisposedException(Strings.MefCompositionContainer_Disposed_Exception);
            }
        }

        /// <summary>
        /// Gets the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="scopedContextExport">The scoped context export.</param>
        /// <returns>
        /// The composition context.
        /// </returns>
        private static ICompositionContext GetOrAddCompositionContext(Export<CompositionContext> scopedContextExport)
        {
            return map.GetOrAdd(scopedContextExport.Value, _ => new MefScopedCompositionContext(scopedContextExport));
        }
    }
}
