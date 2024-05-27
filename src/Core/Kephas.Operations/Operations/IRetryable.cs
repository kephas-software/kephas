// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Marker interface for retryable operations.
/// </summary>
public interface IRetryable
{
    /// <summary>
    /// Gets a value indicating whether the operation that generated the exception can be retried.
    /// </summary>
    public bool CanRetry() => true;
}
