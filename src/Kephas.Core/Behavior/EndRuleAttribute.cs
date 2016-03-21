// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndRuleAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the end rule attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behavior
{
    using System;

    using Kephas.Composition.Metadata;

    /// <summary>
    /// Indicates an end rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EndRuleAttribute : Attribute, IMetadataValue<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndRuleAttribute" /> class.
        /// </summary>
        public EndRuleAttribute()
        {
            this.Value = true;
        }

        /// <summary>
        /// Gets a value indicating whether the rule is an end rule.
        /// </summary>
        /// <value>
        /// Boolean value indicating whether the rule is an end rule.
        /// </value>
        public bool Value { get; }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        /// <value>
        /// The metadata value.
        /// </value>
        object IMetadataValue.Value => this.Value;
    }
}