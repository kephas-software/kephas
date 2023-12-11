// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Priority.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Enumerates the priority values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    /// <summary>
    /// Enumerates the priority values.
    /// They are practically a convenient way to provide integer values for defining priorities.
    /// A lower value indicates a higher priority.
    /// </summary>
    public enum Priority
    {
        /// <summary>
        /// The lowest priority. Typically used by the null services.
        /// </summary>
        Lowest = int.MaxValue,

        /// <summary>
        /// The low priority. Typically used by the default services.
        /// </summary>
        Low = 1000000,

        /// <summary>
        /// The below normal priority. Typically used by services with a higher specialization than the default ones.
        /// </summary>
        BelowNormal = 1000,

        /// <summary>
        /// The normal priority (the default).
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The above normal priority.
        /// </summary>
        AboveNormal = -1000,

        /// <summary>
        /// The high priority.
        /// </summary>
        High = -1000000,

        /// <summary>
        /// The highest priority.
        /// </summary>
        Highest = int.MinValue,
    }
}