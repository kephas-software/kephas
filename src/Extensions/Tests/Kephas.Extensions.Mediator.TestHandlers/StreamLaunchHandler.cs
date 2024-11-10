using System.Runtime.CompilerServices;
using MediatR;

namespace Kephas.Extensions.Mediator.TestHandlers;

public record StreamValidatedLaunchRequest(string Command) : IStreamRequest<StreamLaunchResponse>;

public record StreamLaunchRequest : IStreamRequest<StreamLaunchResponse>;

public record StreamLaunchResponse;

public class StreamLaunchHandler : IStreamRequestHandler<StreamLaunchRequest, StreamLaunchResponse>
{
    public async IAsyncEnumerable<StreamLaunchResponse> Handle(StreamLaunchRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new StreamLaunchResponse();
    }
}

public class StreamValidatedLaunchHandler : IStreamRequestHandler<StreamValidatedLaunchRequest, StreamLaunchResponse>
{
    public async IAsyncEnumerable<StreamLaunchResponse> Handle(StreamValidatedLaunchRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new StreamLaunchResponse();
    }
}
