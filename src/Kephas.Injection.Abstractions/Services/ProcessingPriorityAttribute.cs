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

    using Kephas.Injection;

    /// <summary>
    /// Indicates a processing priority used when more services must be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProcessingPriorityAttribute : Attribute, IMetadataValue<Priority>, IHasProcessingPriority
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingPriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The processing priority.</param>
        public ProcessingPriorityAttribute(int priority)
        {
            this.Value = (Priority)priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingPriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The processing priority.</param>
        public ProcessingPriorityAttribute(Priority priority)
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
        /// Gets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        Priority IHasProcessingPriority.ProcessingPriority => this.Value;
    }
}