// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerilogLevelSwitch.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Serilog.Core;

namespace Kephas.Logging;

/// <summary>
/// Provides the switch for the logging level.
/// </summary>
public interface ISerilogLevelSwitch : ILogLevelSwitch
{
    /// <summary>
    /// Gets the switch for the logging level.
    /// </summary>
    LoggingLevelSwitch LevelSwitch { get; }
}