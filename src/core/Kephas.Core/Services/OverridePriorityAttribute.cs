// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OverridePriorityAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Indicates an override priority for services when more service implementations are defined for the same contract.
//   The service with the highest priority defined will be used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;

    using Kephas.Composition.Metadata;

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
        public Priority Value { get; private set; }

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