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
    /// Gets the UTC timestamp.
    /// </summary>
    DateTime GetUtcDateTime();

    /// <summary>
    /// Gets the UTC timestamp with time zone.
    /// </summary>
    DateTimeOffset GetUtcDateTimeOffset();

    /// <summary>
    /// Gets the local timestamp.
    /// </summary>
    DateTime GetLocalDateTime();

    /// <summary>
    /// Gets the local timestamp with time zone.
    /// </summary>
    DateTimeOffset GetLocalDateTimeOffset();
}
