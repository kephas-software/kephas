// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestExceptionHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Exception handler for middleware 
/// </summary>
[TransientService]
public interface IRequestExceptionHandler
{
    /// <summary>
    /// Handles the request exception.
    /// </summary>
    /// <param name="exceptionContext">The exception context providing the exception to be handled.</param>
    /// <param name="writeResponse">The write response callback.</param>
    /// <returns>A task.</returns>
    Task HandleException(IExceptionContext exceptionContext, Func<int, string, Task> writeResponse);
}