// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndRuleAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the end rule attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;

namespace Kephas.Behaviors
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Indicates an end rule.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class EndRuleAttribute : Attribute, IServiceMetadataValue<bool>
    {
        /// <summary>
        /// Gets a value indicating whether the rule is an end rule.
        /// </summary>
        /// <value>
        /// Boolean value indicating whether the rule is an end rule.
        /// </value>
        public bool Value => true;

        /// <summary>
        /// Gets the metadata name. If the name is not provided, it is inferred from the attribute type name.
        /// </summary>
        string? IServiceMetadataValue.Name => nameof(IBehaviorRuleFlowControl.IsEndRule);
    }
}
