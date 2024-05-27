// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimeProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Time;

/// <summary>
/// Time provider service.
/// </summary>
[SingletonAppServiceContract]
public interface ITimeProvider
{
    /// <summary>
    /// Gets the UTC timestamp with time zone.
    /// </summary>
    DateTimeOffset GetUtcNow();

    /// <summary>
    /// Gets the local timestamp with time zone.
    /// </summary>
    DateTimeOffset GetLocalNow();

    /// <summary>
    /// Gets the timestamp.
    /// </summary>
    /// <returns>A long value representing the current timestamp.</returns>
    long GetTimestamp();

    /// <summary>
    /// Gets the time out of the provided timestamp.
    /// </summary>
    /// <param name="timestamp">The timestamp.</param>
    /// <returns>The corresponding time.</returns>
    DateTimeOffset GetTime(long timestamp);
}
