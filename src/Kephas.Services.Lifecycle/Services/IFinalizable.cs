// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFinalizable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IFinalizable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Provides the <see cref="Finalize"/> method for service finalization.
    /// </summary>
    public interface IFinalizable
    {
        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        void Finalize(IContext? context = null);
    }

    /// <summary>
    /// Provides the <see cref="Finalize"/> method for service finalization.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IFinalizable<in TContext> : IFinalizable
        where TContext : class, IContext
    {
        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        void Finalize(TContext? context = null);

        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        void IFinalizable.Finalize(IContext? context)
        {
            var typedContext = context as TContext;
            if (typedContext == null && context != null)
            {
                throw new ArgumentException($"Expecting a context of type {typeof(TContext)}, instead received {context}.", nameof(context));
            }

            this.Finalize(typedContext);
        }
    }
}