// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextAccessorExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Context;

/// <summary>
/// Extension methods for <see cref="IContextAccessor"/>.
/// </summary>
public static class ContextAccessorExtensions
{
    /// <summary>
    /// Gets the correlation ID.
    /// </summary>
    /// <param name="contextAccessor">The context accessor.</param>
    /// <returns>The correlation ID.</returns>
    public static CorrelationId GetCorrelationId(this IContextAccessor contextAccessor) =>
        contextAccessor.GetOrAddValue(() => new CorrelationId());
}