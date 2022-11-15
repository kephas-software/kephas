// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInitializable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    /// <summary>
    /// Provides the <see cref="Initialize"/> method for service initialization.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void Initialize(IContext? context = null);
    }

    /// <summary>
    /// Provides the <see cref="Initialize"/> method for service initialization.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public interface IInitializable<in TContext> : IInitializable
        where TContext : class, IContext
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void Initialize(TContext? context = null);

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void IInitializable.Initialize(IContext? context)
        {
            var typedContext = context as TContext;
            if (typedContext == null && context != null)
            {
                throw new ArgumentException($"Expecting a context of type {typeof(TContext)}, instead received {context}.", nameof(context));
            }

            this.Initialize(typedContext);
        }
    }
}