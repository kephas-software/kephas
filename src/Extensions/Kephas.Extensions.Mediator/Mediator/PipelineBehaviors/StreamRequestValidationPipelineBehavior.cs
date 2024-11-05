// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamRequestValidationPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Kephas.Extensions.Validation;
using MediatR;

namespace Kephas.Extensions.Mediator.PipelineBehaviors
{
    /// <summary>
    /// Pipeline behavior validating the incoming requests.
    /// </summary>
    /// <typeparam name="TRequest">The request type.</typeparam>
    /// <typeparam name="TResponse">The response type.</typeparam>
    public class StreamRequestValidationPipelineBehavior<TRequest, TResponse>(IValidationService validationService)
        : IStreamPipelineBehavior<TRequest, TResponse>
        where TRequest : IStreamRequest<TResponse>
    {
        /// <summary>
        /// Stream Pipeline handler. Perform any additional behavior and iterate the <paramref name="next" /> delegate as necessary
        /// </summary>
        /// <param name="request">Incoming request</param>
        /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Awaitable task returning the <typeparamref name="TResponse" /></returns>
        public async IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            await validationService.EnsureValidAsync(request, cancellationToken);

            await foreach(var response in next().WithCancellation(cancellationToken))
            {
                yield return response;
            }
        }
    }
}