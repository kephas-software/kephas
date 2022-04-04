// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConditionalActivity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Activities;

using System;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Threading.Tasks;
using Kephas.Workflow;

/// <summary>
/// Activity with conditional execution, corresponding to the if/then/else flow.
/// </summary>
public class ConditionalActivity : FlowActivityBase
{
    /// <summary>
    /// Gets or sets the condition to be evaluated.
    /// </summary>
    public IActivity? Condition { get; set; }

    /// <summary>
    /// Gets or sets the activity to execute if the condition evaluates to true.
    /// </summary>
    public IActivity? Then { get; set; }

    /// <summary>
    /// Gets or sets the activity to execute if the condition evaluates to false.
    /// </summary>
    public IActivity? Else { get; set; }

    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    public override async Task<object?> ExecuteAsync(
        IActivityContext context,
        CancellationToken cancellationToken = default)
    {
        var conditionValue = this.Condition == null
            ? null
            : await this.ExecuteChildActivityAsync(
                this.Condition,
                context,
                this.Target,
                this.Arguments,
                cancellationToken).PreserveThreadContext();
        var boolValue = conditionValue != null && Convert.ToBoolean(conditionValue);
        var activity = boolValue ? this.Then : this.Else;
        return activity == null
            ? null
            : await this.ExecuteChildActivityAsync(
                activity,
                context,
                this.Target,
                this.Arguments,
                cancellationToken).PreserveThreadContext();
    }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    public override IActivity Clone()
        => new ConditionalActivity
        {
            Condition = (this.Condition as ICloneable)?.Clone() as IActivity ?? this.Condition,
            Then = (this.Then as ICloneable)?.Clone() as IActivity ?? this.Then,
            Else = (this.Else as ICloneable)?.Clone() as IActivity ?? this.Else,
        };
}