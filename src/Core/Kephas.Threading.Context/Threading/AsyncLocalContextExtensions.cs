// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLocalContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Diagnostics;

namespace Kephas.Threading;

/// <summary>
/// Extension methods for <see cref="IAsyncLocalContext"/>.
/// </summary>
public static class AsyncLocalContextExtensions
{
    /// <summary>
    /// Gets the correlation ID.
    /// </summary>
    /// <param name="asyncLocalContext">The context accessor.</param>
    /// <returns>The correlation ID.</returns>
    public static CorrelationId GetCorrelationId(this IAsyncLocalContext asyncLocalContext) =>
        asyncLocalContext.GetOrAddValue(() => new CorrelationId());
}