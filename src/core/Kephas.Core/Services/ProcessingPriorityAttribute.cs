// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessingPriorityAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Indicates a processing priority used when more services must be processed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Indicates a processing priority used when more services must be processed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProcessingPriorityAttribute : Attribute, IMetadataValue<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessingPriorityAttribute" /> class.
        /// </summary>
        /// <param name="priority">The override priority.</param>
        public ProcessingPriorityAttribute(int priority)
        {
            this.Value = priority;
        }

        /// <summary>
        /// Gets the priority value.
        /// </summary>
        /// <value>
        /// The priority value.
        /// </value>
        public int Value { get; private set; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value
        {
            get { return this.Value; }
        }
    }
}