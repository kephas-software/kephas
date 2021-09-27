﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndRuleAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the end rule attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors
{
    using System;

    using Kephas.Injection;

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
        /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
        /// </summary>
        string? IMetadataValue.Name => nameof(IBehaviorRuleFlowControl.IsEndRule);
    }
}