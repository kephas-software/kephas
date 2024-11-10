// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetLogLevelRequest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;

namespace Kephas.Web.Endpoints.Admin.Messages;

/// <summary>
/// Request for setting the log level. The response is the current log level after the value has been set.
/// </summary>
/// <param name="LogLevel">The new log level.</param>
public record SetLogLevelRequest(LogLevel LogLevel) : IRequest<LogLevel>;