// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;

namespace Kephas.Time;

/// <summary>
/// The default implementation of the <see cref="ITimeProvider"/> service.
/// </summary>
public class TimeProvider : ITimeProvider
{
    /// <summary>
    /// Gets the UTC timestamp with time zone.
    /// </summary>
    public DateTimeOffset GetUtcNow() => DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the local timestamp with time zone.
    /// </summary>
    public DateTimeOffset GetLocalNow() => DateTimeOffset.Now;

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <returns>A long value representing the current timestamp.</returns>
    public long GetTimestamp() => Stopwatch.GetTimestamp();

    /// <summary>
    /// Gets the time out of the provided timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <returns>The corresponding time.</returns>
    public DateTimeOffset GetTime(long timestamp) => new(new DateTime(timestamp));
}
