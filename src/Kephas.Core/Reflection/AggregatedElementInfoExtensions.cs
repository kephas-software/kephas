// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatedElementInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the aggregated element information extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Linq;

    /// <summary>
    /// Extension methods for <see cref="IAggregatedElementInfo"/>.
    /// </summary>
    public static class AggregatedElementInfoExtensions
    {
        /// <summary>
        /// Indicates whether the <see cref="IAggregatedElementInfo"/> aggregates the provided part at any level.
        /// </summary>
        /// <remarks>The method is <c>null</c> tollerant. If <paramref name="aggregatedElementInfo"/> is <c>null</c>, <c>false</c> is returned.</remarks>
        /// <param name="aggregatedElementInfo">The <see cref="IAggregatedElementInfo"/> to act on.</param>
        /// <param name="part">The part.</param>
        /// <returns>
        /// <c>true</c> if the part is agreggated, <c>false</c> if not.
        /// </returns>
        public static bool Aggregates(this IAggregatedElementInfo aggregatedElementInfo, object part)
        {
            if (aggregatedElementInfo == null)
            {
                return false;
            }

            return aggregatedElementInfo.Parts.Any(p => p == part || (p as IAggregatedElementInfo)?.Aggregates(part) == true);
        }
    }
}