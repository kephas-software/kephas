// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac
{
    using System;
    using System.Collections.Generic;

    using global::Autofac;
    using Kephas.Injection;
    using Kephas.Injection.Autofac.Resources;
    using Kephas.Logging;

    /// <summary>
    /// An Autofac injector base.
    /// </summary>
    public abstract class AutofacInjectorBase : Loggable, IInjector, IServiceProvider
    {
        private readonly IInjectionContainer? root;

        private ILifetimeScope? innerContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInjectorBase"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="logManager">The log manager.</param>
        internal AutofacInjectorBase(IInjectionContainer? root, ILogManager? logManager)
            : base(logManager)
        {
            this.root = root;
        }

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType" />.
        /// </returns>
        public object Resolve(Type contractType)
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
        public IEnumerable<object> ResolveMany(Type contractType)
        {
            this.AssertNotDisposed();

            var collectionContract = typeof(IEnumerable<>).MakeGenericType(contractType ?? throw new ArgumentNullException(nameof(contractType)));

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
        /// <returns>
        /// An object implementing <typeparamref name="T" />.
        /// </returns>
        public T Resolve<T>()
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
        public IEnumerable<T> ResolveMany<T>()
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
        /// <returns>
        /// An object implementing <paramref name="contractType" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object? TryResolve(Type contractType)
        {
            this.AssertNotDisposed();

            return this.innerContainer!.ResolveOptional(contractType);
        }

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <returns>
        /// An object implementing <typeparamref name="T" />, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public T? TryResolve<T>()
            where T : class
        {
            this.AssertNotDisposed();

            return this.innerContainer!.ResolveOptional<T>();
        }

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        public IInjectionScope CreateScope()
        {
            this.AssertNotDisposed();

            var scope = this.innerContainer!.BeginLifetimeScope();
            return (this.root ?? (IInjectionContainer)this).GetInjector(scope);
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.
        /// -or-
        /// <see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</returns>
        /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.IServiceProvider.GetService?view=netstandard-2.1">`IServiceProvider.GetService` on docs.microsoft.com</a></footer>
        object? IServiceProvider.GetService(Type serviceType)
        {
            return this.TryResolve(serviceType);
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

            (this.root ?? (IInjectionContainer)this).HandleDispose(this.innerContainer);
            this.innerContainer.Dispose();
            this.innerContainer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacInjectorBase"/> class.
        /// </summary>
        /// <param name="container">The inner container.</param>
        protected void Initialize(ILifetimeScope container)
        {
            container = container ?? throw new ArgumentNullException(nameof(container));

            this.innerContainer = container;
        }

        /// <summary>
        /// Asserts that the container is not disposed.
        /// </summary>
        protected void AssertNotDisposed()
        {
            if (this.innerContainer == null)
            {
                this.Logger.Error(Strings.AutofacInjector_Disposed_Exception);
                throw new ObjectDisposedException(Strings.AutofacInjector_Disposed_Exception);
            }
        }
    }
}