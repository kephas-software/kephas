// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Defines a base contract for context-dependent operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Security.Principal;

    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IExpando, IAmbientServicesAware, ICompositionContextAware
    {
        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets the context logger.
        /// </summary>
        /// <value>
        /// The context logger.
        /// </value>
        ILogger ContextLogger { get; set; }
    }

    /// <summary>
    /// A context extensions.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// This context.
        /// </returns>
        public static TContext WithIdentity<TContext>(this TContext context, IIdentity identity)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context.Identity = identity;
            return context;
        }
    }
}