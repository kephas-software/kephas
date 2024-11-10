// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLogLevelRequestHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Logging;
using Kephas.Web.Endpoints.Admin.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kephas.Web.Endpoints.Admin.Handlers;

/// <summary>
/// Handler for <see cref="SetLogLevelRequest"/>.
/// </summary>
public class SetLogLevelRequestHandler(
    ILogLevelSwitch logLevelSwitch,
    ILogger<SetLogLevelRequestHandler> logger) : IRequestHandler<SetLogLevelRequest, LogLevel>
{
    /// <summary>Handles the request.</summary>
    /// <returns>The current log level after the value has been set.</returns>
    public Task<LogLevel> Handle(SetLogLevelRequest request, CancellationToken cancellationToken)
    {
        var oldLogeLevel = logLevelSwitch.GetLogLevel();
        logLevelSwitch.SetLogLevel(request.LogLevel);
        var newLogLevel = logLevelSwitch.GetLogLevel();
        logger.LogWarning("Log level changed from {OldLogLevel} to {NewLogLevel}.", oldLogeLevel, newLogLevel);
        return Task.FromResult(newLogLevel);
    }
}