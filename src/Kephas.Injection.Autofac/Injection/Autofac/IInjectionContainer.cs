// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInjectionContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Autofac
{
    using global::Autofac;
    using Kephas.Injection;

    /// <summary>
    /// Interface for injection container.
    /// </summary>
    internal interface IInjectionContainer : IInjector
    {
        /// <summary>
        /// Gets the injector wrapper for the provided injector.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <returns>
        /// The injector.
        /// </returns>
        IInjector GetInjector(ILifetimeScope scope);

        /// <summary>
        /// Tries to get the injector wrapper for the provided injector.
        /// </summary>
        /// <param name="context">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The injector wrapper.
        /// </returns>
        IInjector? TryGetInjector(IComponentContext context, bool createNewIfMissing);

        /// <summary>
        /// Cleanups the given injector.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        void HandleDispose(ILifetimeScope lifetimeScope);
    }
}