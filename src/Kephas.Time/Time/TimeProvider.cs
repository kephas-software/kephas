// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Time;

/// <summary>
/// The default implementation of the <see cref="ITimeProvider"/> service.
/// </summary>
public class TimeProvider : ITimeProvider
{
    /// <summary>
    /// Gets the UTC timestamp.
    /// </summary>
    public DateTime GetUtcDateTime() => DateTime.UtcNow;

    /// <summary>
    /// Gets the UTC timestamp with time zone.
    /// </summary>
    public DateTimeOffset GetUtcDateTimeOffset() => DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the local timestamp.
    /// </summary>
    public DateTime GetLocalDateTime() => DateTime.Now;

    /// <summary>
    /// Gets the local timestamp with time zone.
    /// </summary>
    public DateTimeOffset GetLocalDateTimeOffset() => DateTimeOffset.Now;
}
