// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacServiceProviderBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the autofac injector base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Autofac
{
    using System;

    using global::Autofac;
    using Kephas.Logging;
    using Kephas.Services.Autofac.Resources;

    /// <summary>
    /// An Autofac injector base.
    /// </summary>
    public abstract class AutofacServiceProviderBase : ILoggable, IServiceProvider, IAdapter<ILifetimeScope>
    {
        private readonly IServiceProviderContainer? root;

        private ILifetimeScope? innerContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceProviderBase"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="logger">The logger.</param>
        internal AutofacServiceProviderBase(IServiceProviderContainer? root, ILogger? logger)
        {
            this.root = root;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger? Logger { get; }

        /// <summary>
        /// Gets the object the current instance adapts.
        /// </summary>
        /// <value>
        /// The object the current instance adapts.
        /// </value>
        ILifetimeScope IAdapter<ILifetimeScope>.Of => this.innerContainer!;

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
            GC.SuppressFinalize(this);
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

            (this.root ?? (IServiceProviderContainer)this).HandleDispose(this.innerContainer);
            this.innerContainer.Dispose();
            this.innerContainer = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceProviderBase"/> class.
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