// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnection.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity;

/// <summary>
/// Handles a connection to a specific resource.
/// </summary>
/// <seealso cref="System.IDisposable" />
public interface IConnection : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Disposes connection asynchronously.
    /// </summary>
    /// <returns>The asynchronous result.</returns>
    ValueTask IAsyncDisposable.DisposeAsync()
    {
        return new ValueTask(Task.Factory.StartNew(() => this.Dispose()));
    }
}
