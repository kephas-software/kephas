﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBehaviorRuleFlowControl.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IBehaviorRuleFlowControl interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors
{
    using Kephas.Services;

    /// <summary>
    /// Contract for controlling the flow of behavior rules.
    /// </summary>
    public interface IBehaviorRuleFlowControl : IHasProcessingPriority
    {
        /// <summary>
        /// Gets a value indicating whether this rule ends the processing flow.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rule ends the processing flow, <c>false</c> if not.
        /// </value>
        bool IsEndRule { get; }
    }
}