// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatedElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for aggregated element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Contract for aggregated element information.
    /// </summary>
    public interface IAggregatedElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<object> Parts { get; }

        /// <summary>
        /// Indicates whether the <see cref="IAggregatedElementInfo"/> aggregates the provided part at any level.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <returns>
        /// <c>true</c> if the part is aggregated, <c>false</c> if not.
        /// </returns>
        public bool Aggregates(object part)
        {
            return this.Parts.Any(p => p == part || (p as IAggregatedElementInfo)?.Aggregates(part) == true);
        }
    }
}