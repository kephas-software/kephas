// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoopActivity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Activities;

using Kephas.Threading.Tasks;
using Kephas.Workflow.Interaction;

/// <summary>
/// Activity with repeating execution while a condition is true, corresponding to the while flow.
/// </summary>
public class ConditionalLoopActivity : FlowActivityBase
{
    /// <summary>
    /// Gets or sets the condition to be evaluated.
    /// </summary>
    public IActivity? Condition { get; set; }

    /// <summary>
    /// Gets or sets the activity to execute during one loop.
    /// </summary>
    public IActivity? Do { get; set; }

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
        object? result = null;
        while (true)
        {
            var conditionValue = this.Condition == null
                ? null
                : await this.ExecuteChildActivityAsync(
                    this.Condition,
                    context,
                    this.Target,
                    this.Arguments,
                    cancellationToken).PreserveThreadContext();
            if (conditionValue != null && Convert.ToBoolean(conditionValue) is false)
            {
                break;
            }

            try
            {
                var activity = this.Do;
                result = activity == null
                    ? null
                    : await this.ExecuteChildActivityAsync(
                        activity,
                        context,
                        this.Target,
                        this.Arguments,
                        cancellationToken).PreserveThreadContext();
            }
            catch (BreakLoopSignal)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    public override IActivity Clone()
        => new ConditionalLoopActivity
        {
            Condition = (this.Condition as ICloneable)?.Clone() as IActivity ?? this.Condition,
            Do = (this.Do as ICloneable)?.Clone() as IActivity ?? this.Do,
        };
}