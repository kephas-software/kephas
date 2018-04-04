// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatedElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for aggregated element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;

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
    }
}