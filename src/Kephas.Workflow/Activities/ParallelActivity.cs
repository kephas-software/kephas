// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParallelActivity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Activities;

using Kephas.Threading.Tasks;

/// <summary>
/// Defines a block activity executing the child activities in parallel.
/// </summary>
public class ParallelActivity : BlockActivityBase
{
    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    public override async Task<object?> ExecuteAsync(IActivityContext context, CancellationToken cancellationToken = default)
    {
        if (this.Activities == null)
        {
            return null;
        }

        var tasks = this.Activities.Select(activity => this.ExecuteChildActivityAsync(activity, context, this.Target, this.Arguments, cancellationToken));
        return await Task.WhenAll(tasks).PreserveThreadContext();
    }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    public override IActivity Clone()
        => new ParallelActivity { Activities = this.Activities?.Select(a => (a as ICloneable)?.Clone() as IActivity ?? a) };
}
