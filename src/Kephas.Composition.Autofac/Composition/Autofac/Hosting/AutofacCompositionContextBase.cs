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

    using Kephas.Composition.Autofac.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;

    /// <summary>
    /// An Autofac composition context base.
    /// </summary>
    public abstract class AutofacCompositionContextBase : ICompositionContext
    {
        private readonly ICompositionContainer? root;

        private ILifetimeScope? innerContainer;
        private ILogger? logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacCompositionContextBase"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        internal AutofacCompositionContextBase(ICompositionContainer? root)
        {
            this.root = root;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected ILogger? Logger => this.logger ??= this.GetLogger(this.innerContainer);

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />.
        /// </returns>
        public object GetExport(Type contractType, string? serviceName = null)
        {
            this.AssertNotDisposed();

            if (this.Logger.IsDebugEnabled())
            {
                try
                {
                    return this.innerContainer!.Resolve(contractType);
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while resolving service {service}.", contractType);
                    throw;
                }
            }

            return this.innerContainer!.Resolve(contractType);
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An enumeration of objects implementing <paramref name="contractType" />.
        /// </returns>
        public IEnumerable<object> GetExports(Type contractType)
        {
            this.AssertNotDisposed();

            var collectionContract = typeof(IEnumerable<>).MakeGenericType(contractType);

            if (this.Logger.IsDebugEnabled())
            {
                try
                {
                    return (IEnumerable<object>)this.innerContainer!.Resolve(collectionContract);
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while resolving service {service}.", collectionContract);
                    throw;
                }
            }

            return (IEnumerable<object>)this.innerContainer!.Resolve(collectionContract);
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T GetExport<T>(string? serviceName = null)
            where T : class
        {
            this.AssertNotDisposed();

            if (this.Logger.IsDebugEnabled())
            {
                try
                {
                    return this.innerContainer!.Resolve<T>();
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while resolving service {service}.", typeof(T));
                    throw;
                }
            }

            return this.innerContainer!.Resolve<T>();
        }

        /// <summary>
        /// Resolves the specified contract type returning multiple instances.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An enumeration of objects implementing <typeparamref name="T" />.
        /// </returns>
        public IEnumerable<T> GetExports<T>()
            where T : class
        {
            this.AssertNotDisposed();

            if (this.Logger.IsDebugEnabled())
            {
                try
                {
                    return this.innerContainer!.Resolve<IEnumerable<T>>();
                }
                catch (Exception ex)
                {
                    this.Logger.Debug(ex, "Error while resolving service {service}.", typeof(IEnumerable<T>));
                    throw;
                }
            }

            return this.innerContainer!.Resolve<IEnumerable<T>>();
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object? TryGetExport(Type contractType, string? serviceName = null)
        {
            this.AssertNotDisposed();

            return this.innerContainer!.ResolveOptional(contractType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceName">The service name.</param>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T? TryGetExport<T>(string? serviceName = null)
            where T : class
        {
            this.AssertNotDisposed();

            return this.innerContainer!.ResolveOptional<T>();
        }

        /// <summary>
        /// Creates a new scoped composition context.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public ICompositionContext CreateScopedContext()
        {
            this.AssertNotDisposed();

            var scope = this.innerContainer!.BeginLifetimeScope();
            return (this.root ?? (ICompositionContainer)this).GetCompositionContext(scope);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
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

            (this.root ?? (ICompositionContainer)this).HandleDispose(this.innerContainer);
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
        /// Gets the logger from the provided container.
        /// </summary>
        /// <param name="container">The container used to get the logger.</param>
        /// <returns>The logger.</returns>
        protected ILogger? GetLogger(ILifetimeScope? container)
        {
            return container == null
                ? null
                : container.TryResolve<ILogManager>(out var logManager)
                    ? logManager.GetLogger(this.GetType())
                    : this.GetType().GetLogger();
        }
    }
}