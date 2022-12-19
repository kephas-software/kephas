// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasOverridePriority.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="OverridePriority"/> property.
    /// </summary>
    public interface IHasOverridePriority
    {
        /// <summary>
        /// Gets the override priority.
        /// </summary>
        /// <value>
        /// The override priority.
        /// </value>
        public Priority OverridePriority { get; }
    }
}