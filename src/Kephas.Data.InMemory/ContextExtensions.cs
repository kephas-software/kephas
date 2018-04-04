// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
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
        /// Gets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <returns>
        /// An enumeration of entity information.
        /// </returns>
        public static IEnumerable<IEntityInfo> GetInitialData(this IContext context)
        {
            return context?[nameof(InMemoryDataContextConfiguration.InitialData)] as IEnumerable<IEntityInfo>;
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data.</param>
        public static void SetInitialData(this IContext context, IEnumerable<Tuple<object, ChangeState>> initialData)
        {
            Requires.NotNull(context, nameof(context));

            context[nameof(InMemoryDataContextConfiguration.InitialData)] =
                    initialData
                        ?.Where(t => t.Item1 != null)
                        .Select(t => new EntityInfo(t.Item1) { ChangeState = t.Item2 });
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data as an enumeration of entities.</param>
        public static void SetInitialData(this IContext context, IEnumerable<object> initialData)
        {
            Requires.NotNull(context, nameof(context));

            context[nameof(InMemoryDataContextConfiguration.InitialData)] =
                    initialData
                        ?.Where(o => o != null)
                        .Select(o => new EntityInfo(o));
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data as an enumeration of entities.</param>
        public static void SetInitialData(this IContext context, IEnumerable<IEntityInfo> initialData)
        {
            Requires.NotNull(context, nameof(context));

            context[nameof(InMemoryDataContextConfiguration.InitialData)] = initialData?.Where(e => e != null);
        }
    }
}