using Kephas.DependencyInjection.Metadata;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Mediator.PipelineBehaviors;

/// <summary>
/// Pipeline behavior used to do request handling logging.
/// </summary>
/// <param name="logger">The logger.</param>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
[Priority(Priority.High)]
public class StreamLogPipelineBehavior<TRequest, TResponse>(ILogger<StreamLogPipelineBehavior<TRequest, TResponse>> logger)
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
    public IAsyncEnumerable<TResponse> Handle(TRequest request, StreamHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug("Handling stream request: {Request}", request);
        }

        try
        {
            var response = next();

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Handled stream request successfully: {Request}. Response: {Response}", request, response);
            }

            return response;
        }
        catch (Exception ex)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug(ex, "Handling stream request failed: {Request}", request);
            }

            throw;
        }
    }
}