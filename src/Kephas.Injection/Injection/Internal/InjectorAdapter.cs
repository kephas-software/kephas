// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectorAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the injector adapter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Internal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An adapter of <see cref="IServiceProvider"/> for <see cref="IInjector"/>.
    /// </summary>
    internal class InjectorAdapter : IInjector, IAdapter<IServiceProvider>
    {
        private readonly IServiceProvider serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InjectorAdapter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public InjectorAdapter(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the object the current instance adapts.
        /// </summary>
        /// <value>
        /// The object the current instance adapts.
        /// </value>
        public IServiceProvider Of => this.serviceProvider;

        /// <summary>
        /// Resolves the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType"/>.
        /// </returns>
        public object Resolve(Type contractType) =>
            this.serviceProvider.GetRequiredService(contractType ?? throw new ArgumentNullException(nameof(contractType)));

        /// <summary>
        /// Tries to resolve the specified contract type.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>
        /// An object implementing <paramref name="contractType"/>, or <c>null</c> if a service with the
        /// provided contract was not found.
        /// </returns>
        public object? TryResolve(Type contractType) =>
            this.serviceProvider.GetService(contractType ?? throw new ArgumentNullException(nameof(contractType)));

        /// <summary>
        /// Creates a new scoped injector.
        /// </summary>
        /// <returns>
        /// The new scoped context.
        /// </returns>
        IInjectionScope IInjector.CreateScope()
        {
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        void IDisposable.Dispose()
        {
        }
    }
}