// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasProcessingPriority.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Provides the <see cref="ProcessingPriority"/> property.
    /// </summary>
    public interface IHasProcessingPriority
    {
        /// <summary>
        /// Gets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public Priority ProcessingPriority { get; }
    }
}