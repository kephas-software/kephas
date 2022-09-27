// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceProviderContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac
{
    using global::Autofac;

    /// <summary>
    /// Interface for a container or <see cref="IServiceProvider"/>.
    /// </summary>
    internal interface IServiceProviderContainer : IServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> adapter for the provided injector.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <returns>
        /// The injector.
        /// </returns>
        IServiceProvider GetServiceProvider(ILifetimeScope scope);

        /// <summary>
        /// Cleanups the given injector.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        void HandleDispose(ILifetimeScope lifetimeScope);
    }
}