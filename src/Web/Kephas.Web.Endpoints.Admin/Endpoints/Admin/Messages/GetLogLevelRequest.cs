// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetLogLevelRequest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using MediatR;
using Microsoft.Extensions.Logging;

namespace Kephas.Web.Endpoints.Admin.Messages;

/// <summary>
/// Request for getting the log level.
/// </summary>
public record GetLogLevelRequest : IRequest<LogLevel>;