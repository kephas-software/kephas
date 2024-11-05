using MediatR;

namespace Kephas.Extensions.Mediator.TestHandlers;

public record ValidatedLaunchRequest(string Command) : IRequest<LaunchResponse>;

public record LaunchRequest : IRequest<LaunchResponse>;

public record LaunchResponse;

public class LaunchHandler : IRequestHandler<LaunchRequest, LaunchResponse>
{
    public Task<LaunchResponse> Handle(LaunchRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new LaunchResponse());
    }
}

public class ValidatedLaunchHandler : IRequestHandler<ValidatedLaunchRequest, LaunchResponse>
{
    public Task<LaunchResponse> Handle(ValidatedLaunchRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new LaunchResponse());
    }
}
