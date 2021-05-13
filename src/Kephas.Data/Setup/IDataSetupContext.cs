// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSetupContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSetupContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for data setup context.
    /// </summary>
    public interface IDataSetupContext : IContext
    {
        /// <summary>
        /// Gets or sets the data targets.
        /// </summary>
        /// <value>
        /// The data targets.
        /// </value>
        IEnumerable<string>? Targets { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataSetupContext"/>.
    /// </summary>
    public static class DataSetupContextExtensions
    {
        /// <summary>
        /// Sets the data targets.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The data setup context.</param>
        /// <param name="targets">The targets.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TContext Targets<TContext>(this TContext context, IEnumerable<string> targets)
            where TContext : class, IDataSetupContext
        {
            Requires.NotNull(context, nameof(context));
            Requires.NotNull(targets, nameof(targets));

            context.Targets = targets;

            return context;
        }
    }
}