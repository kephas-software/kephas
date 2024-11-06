// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionResultFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.Web.ExceptionHandling.ExceptionResultProviders;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Default implementation of the <see cref="IExceptionResultFactory"/>.
/// </summary>
public class ExceptionResultFactory(ILazyEnumerable<IExceptionResultProvider, ExceptionResultProviderMetadata> lazyProviders)
    : IExceptionResultFactory
{
    private IExceptionResultProvider GetExceptionResultProvider(Exception exception)
    {
        var provider = lazyProviders.GetService(
                           m => m.ExceptionType == exception.GetType())
                       ?? lazyProviders.GetService(
                           m => m.ExceptionType?.IsAssignableFrom(exception.GetType()) ?? false)
                       ?? GenericExceptionResultProvider.Instance;
        return provider;
    }

    /// <summary>
    /// Gets the exception result out of the provided <paramref name="exceptionContext"/>.
    /// </summary>
    /// <param name="exceptionContext">The exception context providing the exception to be handled.</param>
    /// <returns>The exception result.</returns>
    public virtual ExceptionResult GetExceptionResult(IExceptionContext exceptionContext)
    {
        var exception = exceptionContext.Exception;
        var provider = GetExceptionResultProvider(exception);

        var result = provider.TryGetExceptionResult(exceptionContext)
                     ?? GenericExceptionResultProvider.Instance.TryGetExceptionResult(exceptionContext)!;

        return result with
        {
            Url = result.Url ?? exceptionContext.RequestPath,
            Message = result.Message ?? exception.Message,
            InnerException = result.InnerException ?? (exception.InnerException is null ? null : GetExceptionResult(exceptionContext.Clone(exception.InnerException))),
        };
    }
}