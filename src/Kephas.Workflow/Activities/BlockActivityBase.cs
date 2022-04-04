// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlockActivityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Activities;

using System.Collections.Generic;

using Kephas.Workflow;

/// <summary>
/// Base class for a block of activities.
/// </summary>
public abstract class BlockActivityBase : FlowActivityBase
{
    /// <summary>
    /// Gets or sets the block activities.
    /// </summary>
    public IEnumerable<IActivity>? Activities { get; set; }
}