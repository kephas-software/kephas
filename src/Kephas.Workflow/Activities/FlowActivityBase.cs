// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlowActivityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Activities;

using System;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Collections;
using Kephas.Dynamic;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Threading.Tasks;
using Kephas.Workflow;

/// <summary>
/// Base class for flow activities.
/// </summary>
public abstract class FlowActivityBase : ActivityBase, IOperation, ICloneable
{
    /// <summary>
    /// Executes the operation in the given context.
    /// </summary>
    /// <param name="context">Optional. The context.</param>
    /// <returns>
    /// An object.
    /// </returns>
    object? IOperation.Execute(IContext? context) =>
        this.ExecuteAsync(this.GetActivityContext(context)).GetResultNonLocking();

    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">Optional. The context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    Task<object?> IOperation.ExecuteAsync(IContext? context, CancellationToken cancellationToken) =>
        this.ExecuteAsync(this.GetActivityContext(context), cancellationToken);

    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">The activity context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    public abstract Task<object?> ExecuteAsync(IActivityContext context, CancellationToken cancellationToken = default);

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    object ICloneable.Clone() => this.Clone();

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    /// <footer><a href="https://docs.microsoft.com/en-us/dotnet/api/System.ICloneable.Clone?view=netstandard-2.1">`ICloneable.Clone` on docs.microsoft.com</a></footer>
    public abstract IActivity Clone();

    /// <summary>
    /// Makes sure the provided context is an activity context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>An <see cref="IActivityContext"/>.</returns>
    protected virtual IActivityContext GetActivityContext(IContext? context)
    {
        return context as IActivityContext
               ?? throw new InvalidOperationException("Must provide an activity context.");
    }

    /// <summary>
    /// Executes the child activity asynchronously.
    /// </summary>
    /// <param name="childActivity">The child activity.</param>
    /// <param name="parentContext">The parent context.</param>
    /// <param name="target">The target.</param>
    /// <param name="args">The arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>
    /// The child activity result.
    /// </returns>
    protected virtual async Task<object?> ExecuteChildActivityAsync(IActivity childActivity, IActivityContext parentContext, object? target, IDynamic? args, CancellationToken cancellationToken = default)
    {
        var result = await parentContext.WorkflowProcessor
            .ExecuteAsync(
                childActivity,
                target,
                args,
                ctx => ctx
                    .Impersonate(parentContext)
                    .AddResource(new CopyScopeBack(parentContext, ctx, this.ShouldCopyAllScopeFromChild(childActivity, parentContext)))
                    .Scope.Merge(parentContext.Scope),
                cancellationToken)
            .PreserveThreadContext();
        return result;
    }

    /// <summary>
    /// Indicates whether it should copy all scope from child when returning from child execution.
    /// </summary>
    /// <param name="childActivity">The child activity.</param>
    /// <param name="context">The context.</param>
    /// <returns>A boolean value indicating whether it should copy all scope from child when returning from call.</returns>
    protected virtual bool ShouldCopyAllScopeFromChild(IActivity childActivity, IActivityContext context)
    {
        // TODO make this through metadata.
        return childActivity is IParentScopeModifierActivity;
    }

    private record CopyScopeBack(IActivityContext parentContext, IActivityContext childContext, bool allFromChild) : IDisposable
    {
        public void Dispose()
        {
            // copy back only the variables from the parent scope, not the newly created ones.
            var parentScope = this.parentContext.Scope;
            var childScope = this.childContext.Scope;
            (this.allFromChild ? childScope : parentScope)
                .ToDictionary().Keys
                .ForEach(k => parentScope[k] = childScope[k]);
        }
    }
}
