// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelRequestHandler.cs" company="Kephas Software SRL">
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
/// Handler for <see cref="GetLogLevelRequest"/>.
/// </summary>
public class GetLogLevelRequestHandler(ILogLevelSwitch logLevelSwitch) : IRequestHandler<GetLogLevelRequest, LogLevel>
{
    /// <summary>Handles the request.</summary>
    /// <returns>The current log level after the value has been set.</returns>
    public Task<LogLevel> Handle(GetLogLevelRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(logLevelSwitch.GetLogLevel());
}