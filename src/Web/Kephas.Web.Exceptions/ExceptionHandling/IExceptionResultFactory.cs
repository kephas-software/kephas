// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExceptionResultFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Factory service for <see cref="ExceptionResult"/>.
/// </summary>
[SingletonService]
public interface IExceptionResultFactory
{
    /// <summary>
    /// Gets the exception result out of the provided <paramref name="exceptionContext"/>.
    /// </summary>
    /// <param name="exceptionContext">The exception context providing the exception to be handled.</param>
    /// <returns>The exception result.</returns>
    ExceptionResult GetExceptionResult(IExceptionContext exceptionContext);
}