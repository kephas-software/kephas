// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLocalAccessorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading;

/// <summary>
/// Accessor class for an async call local resource.
/// </summary>
/// <typeparam name="T">The local resource type.</typeparam>
public abstract class AsyncLocalAccessorBase<T>
    where T : class
{
    private static readonly AsyncLocal<AsyncDataHolder> Current = new();

    /// <summary>
    /// Gets the user information for the current asynchronous context, if it is available.
    /// </summary>
    protected T? Data
    {
        get => Current.Value?.Data;
        set
        {
            var holder = Current.Value;
            if (holder != null)
            {
                // Clear current data trapped in the AsyncLocals, as its done.
                holder.Data = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the data in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                Current.Value = new AsyncDataHolder { Data = value};
            }
        }
    }

    private sealed record AsyncDataHolder
    {
        public T? Data;
    }
}
