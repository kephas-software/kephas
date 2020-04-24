// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Capabilities;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Extension methods for <see cref="IContext"/>.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Name of the initial data configuration.
        /// </summary>
        public const string InitialDataConfigurationName = "InitialData";

        /// <summary>
        /// Gets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>
        /// An enumeration of entity entry.
        /// </returns>
        public static IEnumerable<IChangeStateTrackableEntityEntry>? InitialData(this IContext? context)
        {
            return context?[InitialDataConfigurationName] as IEnumerable<IChangeStateTrackableEntityEntry>;
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data.</param>
        /// <returns>
        /// This context.
        /// </returns>
        public static TContext InitialData<TContext>(this TContext context, IEnumerable<(object entity, ChangeState changeState)> initialData)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context[InitialDataConfigurationName] =
                initialData
                    ?.Where(t => t.entity != null)
                    .Select(t => new EntityEntry(t.entity) { ChangeState = t.changeState });
            return context;
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data as an enumeration of entities.</param>
        /// <returns>
        /// This context.
        /// </returns>
        public static TContext InitialData<TContext>(this TContext context, IEnumerable<object> initialData)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context[InitialDataConfigurationName] =
                    initialData
                        ?.Where(o => o != null)
                        .Select(o => new EntityEntry(o));
            return context;
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data as an enumeration of entities.</param>
        /// <returns>
        /// This context.
        /// </returns>
        public static TContext InitialData<TContext>(this TContext context, IEnumerable<IChangeStateTrackableEntityEntry> initialData)
            where TContext : class, IContext
        {
            Requires.NotNull(context, nameof(context));

            context[InitialDataConfigurationName] = initialData?.Where(e => e != null);
            return context;
        }
    }
}