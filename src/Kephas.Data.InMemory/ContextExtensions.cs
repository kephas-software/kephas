// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory
{
    using System.Collections.Generic;

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
            Requires.NotNull(context, nameof(context));

            return context[nameof(InMemoryDataContextConfiguration.InitialData)] as IEnumerable<IEntityInfo>;
        }

        /// <summary>
        /// Sets the initial data.
        /// </summary>
        /// <param name="context">The context to act on.</param>
        /// <param name="initialData">The initial data.</param>
        public static void SetInitialData(this IContext context, IEnumerable<IEntityInfo> initialData)
        {
            Requires.NotNull(context, nameof(context));

            context[nameof(InMemoryDataContextConfiguration.InitialData)] = initialData;
        }
    }
}