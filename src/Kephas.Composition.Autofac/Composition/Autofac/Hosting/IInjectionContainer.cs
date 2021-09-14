// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ICompositionContainer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Autofac.Hosting
{
    using global::Autofac;

    /// <summary>
    /// Interface for composition container.
    /// </summary>
    internal interface IInjectionContainer : IInjector
    {
        /// <summary>
        /// Gets the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="scope">The lifetime scope.</param>
        /// <returns>
        /// The composition context.
        /// </returns>
        IInjector GetCompositionContext(ILifetimeScope scope);

        /// <summary>
        /// Tries to get the composition context wrapper for the provided composition context.
        /// </summary>
        /// <param name="context">The inner container.</param>
        /// <param name="createNewIfMissing">True to create new if missing.</param>
        /// <returns>
        /// The composition context wrapper.
        /// </returns>
        IInjector? TryGetCompositionContext(IComponentContext context, bool createNewIfMissing);

        /// <summary>
        /// Cleanups the given composition context.
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope.</param>
        void HandleDispose(ILifetimeScope lifetimeScope);
    }
}