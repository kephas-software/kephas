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
    using System.Collections.Generic;

    using global::Autofac;

    /// <summary>
    /// An Autofac composition context base.
    /// </summary>
    public abstract class AutofacCompositionContextBase : ICompositionContext
    {
        private readonly ILifetimeScope container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContextBase"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        protected AutofacCompositionContextBase(ILifetimeScope container)
        {
            this.container = container;
        }

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
            return this.container.Resolve(contractType);
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
            var collectionContract = typeof(IEnumerable<>).MakeGenericType(contractType);
            return (IEnumerable<object>)this.container.Resolve(collectionContract);
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
            return this.container.Resolve<T>();
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
            return this.container.Resolve<IEnumerable<T>>();
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
            if (this.container.TryResolve(contractType, out var service))
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
            if (this.container.TryResolve<T>(out var service))
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
            return new AutofacScopedCompositionContext(this.container.BeginLifetimeScope(scopeName));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.container.Dispose();
        }
    }
}