// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverridePriorityAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Indicates an override priority for services when more service implementations are defined for the same contract.
//   The service with the highest priority defined will be used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Injection;

    /// <summary>
    /// Indicates an override priority for services when more service implementations are defined for the same contract.
    /// The service with the highest priority defined will be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class OverridePriorityAttribute : Attribute, IMetadataValue<Priority>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverridePriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The override priority.</param>
        public OverridePriorityAttribute(int priority)
        {
            this.Value = (Priority)priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverridePriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The override priority.</param>
        public OverridePriorityAttribute(Priority priority)
        {
            this.Value = priority;
        }

        /// <summary>
        /// Gets the priority value.
        /// </summary>
        /// <value>
        /// The priority value.
        /// </value>
        public Priority Value { get; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;
    }
}