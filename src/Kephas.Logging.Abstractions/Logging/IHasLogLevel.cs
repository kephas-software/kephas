// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasLogLevel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging;

/// <summary>
/// Provides the <see cref="LogLevel"/> property.
/// </summary>
public interface IHasLogLevel
{
    /// <summary>
    /// Gets the log level.
    /// </summary>
    LogLevel LogLevel { get; }
}