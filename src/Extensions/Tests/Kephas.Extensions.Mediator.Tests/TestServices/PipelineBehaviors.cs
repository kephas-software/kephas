// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PipelineBehaviors.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Tests.TestServices;

public abstract class TestPipelineBehaviorBase<TRequest>
    : IPipelineBehavior<TRequest, string>
    where TRequest : IRequest<string>
{
    public async Task<string> Handle(TRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        $"{GetType().Name} > {await next()}";
}

[Priority(Priority.High)]
public class ReturnStringTypedBehavior : TestPipelineBehaviorBase<IReturnStringMessage>;

public class OneReturnStringTypedBehavior : TestPipelineBehaviorBase<OneReturnStringTypedMessage>;

[Priority(Priority.Lowest)]
internal class ReturnStringBehavior : TestPipelineBehaviorBase<IRequest<string>>;

internal class ReturnIntBehavior : IPipelineBehavior<ReturnIntMessage, int>
{
    public async Task<int> Handle(ReturnIntMessage request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken) =>
        await next() + GetType().Name.Length;
}

[Priority(Priority.AboveNormal)]
public class OpenGenericBehavior<TRequest, TResponse>(ILogger<OpenGenericBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (typeof(TResponse) == typeof(string))
        {
            return (TResponse)(object)$"{GetType().Name} > {await next()}";
        }

        logger.LogInformation(GetType().Name);
        return await next();
    }
}
