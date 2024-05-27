// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncOperation.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Defines the contract of an executable operation.
/// </summary>
public interface IAsyncOperation
{
    /// <summary>
    /// Executes the operation asynchronously in the given context.
    /// </summary>
    /// <param name="context">Optional. The context.</param>
    /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
    /// <returns>
    /// An object.
    /// </returns>
    Task<object?> ExecuteAsync(object? context = null, CancellationToken cancellationToken = default);
}
