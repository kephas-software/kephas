// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestExceptionHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Serialization.Json;
using Kephas.Web.ExceptionHandling.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// The default implementation of the <see cref="IRequestExceptionHandler"/>.
/// </summary>
public class RequestExceptionHandler(
    IJsonService serializationService,
    IExceptionResultFactory resultProviderFactory,
    IOptions<ExceptionHandlerSettings> exceptionHandlerSettings,
    ILogger<RequestExceptionHandler> logger) : IRequestExceptionHandler
{
    /// <summary>
    /// Handles the request exception.
    /// </summary>
    /// <param name="exceptionContext">The exception context providing the exception to be handled.</param>
    /// <param name="writeResponse">The write response callback.</param>
    /// <returns>A task.</returns>
    public Task HandleException(IExceptionContext exceptionContext, Func<int, string, Task> writeResponse)
    {
        var result = resultProviderFactory.GetExceptionResult(exceptionContext);
        result.StatusCode ??= (int) exceptionHandlerSettings.Value.DefaultErrorStatusCode;
        
        logger.Log(result.Severity as LogLevel? ?? LogLevel.Error, exceptionContext.Exception, "[Correlation ID: {CorrelationId}, result: {Result}]", result.CorrelationId, result);

        var serializedResult = serializationService.Serialize(result);

        return writeResponse(result.StatusCode.Value, serializedResult);
    }
}
