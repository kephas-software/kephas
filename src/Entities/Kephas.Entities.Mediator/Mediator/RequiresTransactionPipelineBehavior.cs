// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequiresTransactionPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using MediatR;

namespace Kephas.Entities.Mediator;

/// <summary>
/// Behavior for ensuring transactional context of a mediator request handing.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class RequiresTransactionPipelineBehavior<TRequest, TResponse>(IWriteRepository repository)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Awaitable task returning the <typeparamref name="TResponse" /></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var requiresTransactionAttribute = requestType.GetCustomAttribute<RequiresTransactionAttribute>();
        if (requiresTransactionAttribute is null)
        {
            return await next();
        }

        return requiresTransactionAttribute.StartNewTransaction
            ? await repository.EnterNewTransactionAsync((_, _) => next(), cancellationToken)
            : await repository.EnterTransactionAsync((_, _) => next(), cancellationToken);
    }
}