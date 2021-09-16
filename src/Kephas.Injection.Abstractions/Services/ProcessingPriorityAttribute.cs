// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingPriorityAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Indicates a processing priority used when more services must be processed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Injection.Metadata;

    /// <summary>
    /// Indicates a processing priority used when more services must be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProcessingPriorityAttribute : Attribute, IMetadataValue<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingPriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The processing priority.</param>
        public ProcessingPriorityAttribute(int priority)
        {
            this.Value = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingPriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The processing priority.</param>
        public ProcessingPriorityAttribute(Priority priority)
        {
            this.Value = (int)priority;
        }

        /// <summary>
        /// Gets the priority value.
        /// </summary>
        /// <value>
        /// The priority value.
        /// </value>
        public int Value { get; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;
    }
}