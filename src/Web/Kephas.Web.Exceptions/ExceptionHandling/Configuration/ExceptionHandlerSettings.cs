// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlerSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Kephas.Web.ExceptionHandling.Configuration;

/// <summary>
/// Provides settings for the exception handling.
/// </summary>
public class ExceptionHandlerSettings : ExceptionHandlerOptions
{
    /// <summary>
    /// Gets or sets the default error status code.
    /// </summary>
    public HttpStatusCode DefaultErrorStatusCode { get; set; } = HttpStatusCode.InternalServerError;
}