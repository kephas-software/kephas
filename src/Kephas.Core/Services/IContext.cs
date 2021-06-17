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
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Security.Principal;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Logging;

    /// <summary>
    /// Defines a base contract for context-dependent operations.
    /// </summary>
    public interface IContext : IExpandoBase, ILoggable, IDisposable
    {
        /// <summary>
        /// Gets a context for the dependency injection/composition.
        /// </summary>
        /// <value>
        /// The composition context.
        /// </value>
        ICompositionContext CompositionContext { get; }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets or sets the authenticated identity.
        /// </summary>
        /// <value>
        /// The authenticated identity.
        /// </value>
        IIdentity? Identity { get; set; }
    }

    /// <summary>
    /// A context extensions.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// The disposable resources key.
        /// </summary>
        private const string DisposableResourcesKey = "__DisposableResources";

        /// <summary>
        /// Sets the provided value.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Set<TContext>(this TContext context, string key, object? value)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context[key] = value;

            return context;
        }

        /// <summary>
        /// Sets the context identity.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="parentContext">The parent context.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Impersonate<TContext>(this TContext context, IContext? parentContext)
            where TContext : class, IContext
        {
            if (parentContext != null)
            {
                context.Impersonate(parentContext.Identity);
            }

            return context;
        }

        /// <summary>
        /// Sets the context identity.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Impersonate<TContext>(this TContext context, IIdentity? identity)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context.Identity = identity;
            return context;
        }

        /// <summary>
        /// Sets the context logger.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="contextLogger">The context logger.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Logger<TContext>(this TContext context, ILogger contextLogger)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            if (context is Context loggableContext)
            {
                loggableContext.Logger = contextLogger;
            }
            else
            {
                context.SetPropertyValue(nameof(ILoggable.Logger), contextLogger);
            }

            return context;
        }

        /// <summary>
        /// Registers with the context disposable resources to be disposed together with the context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="resources">The resources to add.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext AddResource<TContext>(this TContext context, params IDisposable[] resources)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));
            Requires.NotNull(resources, nameof(resources));

            if (!(context[DisposableResourcesKey] is IList<IDisposable> resourcesList))
            {
                resourcesList = new List<IDisposable>(resources);
                context[DisposableResourcesKey] = resourcesList;
            }
            else
            {
                resourcesList.AddRange(resources);
            }

            return context;
        }

        /// <summary>
        /// Disposes all resources registered with the context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        internal static TContext DisposeResources<TContext>(this TContext context)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            if (context[DisposableResourcesKey] is IList<IDisposable> resources)
            {
                foreach (var resource in resources)
                {
                    resource.Dispose();
                }

                resources.Clear();
            }

            return context;
        }
    }
}