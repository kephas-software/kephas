// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Priority.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// The lowest priority.
        /// </summary>
        Lowest = int.MaxValue,

        /// <summary>
        /// The low priority.
        /// </summary>
        Low = 1000000,

        /// <summary>
        /// The below normal priority.
        /// </summary>
        BelowNormal = 1000,

        /// <summary>
        /// The normal priority.
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