// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExceptionResultProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Provider for an exception result.
/// </summary>
public interface IExceptionResultProvider
{
    /// <summary>
    /// Tries to get the exception result.
    /// </summary>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    ExceptionResult? TryGetExceptionResult(IExceptionContext exceptionContext);
}

/// <summary>
/// Provider for an exception result based on a specific exception type.
/// </summary>
/// <typeparam name="TException">The exception type.</typeparam>
[SingletonService(ServiceType = typeof(IExceptionResultProvider), MetadataType = typeof(ExceptionResultProviderMetadata))]
public interface IExceptionResultProvider<in TException> : IExceptionResultProvider
{
    /// <summary>
    /// Gets the exception result.
    /// </summary>
    /// <param name="exception">The typed exception retrieved from the exception context.</param>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    ExceptionResult? TryGetExceptionResult(TException exception, IExceptionContext exceptionContext);

    /// <summary>
    /// Gets the exception result.
    /// </summary>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns></returns>
    ExceptionResult? IExceptionResultProvider.TryGetExceptionResult(IExceptionContext exceptionContext)
    {
        if (exceptionContext.Exception is not TException typedException)
        {
            throw new ArgumentException($"The provided {nameof(exceptionContext)}.{nameof(exceptionContext.Exception)} must be of type {typeof(TException)}.", nameof(exceptionContext));
        }

        return TryGetExceptionResult(typedException, exceptionContext);
    }
}