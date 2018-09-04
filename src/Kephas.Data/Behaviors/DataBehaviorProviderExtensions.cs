﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBehaviorProviderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data behavior provider extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Collections.Generic;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for <see cref="IDataBehaviorProvider"/>.
    /// </summary>
    public static class DataBehaviorProviderExtensions
    {
        /// <summary>
        /// Gets the data behaviors of type <typeparamref name="TBehavior"/> for the provided object.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="behaviorProvider">The <see cref="IDataBehaviorProvider"/> service.</param>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// An enumeration of behaviors mathing the provided type.
        /// </returns>
        public static IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(
            this IDataBehaviorProvider behaviorProvider,
            object obj)
        {
            Requires.NotNull(behaviorProvider, nameof(behaviorProvider));

            if (obj == null)
            {
                return new TBehavior[0];
            }

            return behaviorProvider.GetDataBehaviors<TBehavior>(obj.GetType());
        }
    }
}