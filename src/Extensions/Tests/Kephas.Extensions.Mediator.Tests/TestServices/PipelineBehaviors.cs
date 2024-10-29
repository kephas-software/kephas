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
    : PipelineBehaviorBase<TRequest, string>
    where TRequest : IRequest<string>
{
    public override async Task<string> Handle(TRequest request, RequestHandlerDelegate<string> next, CancellationToken cancellationToken) =>
        $"{GetType().Name} > {await next()}";
}

[Priority(Priority.High)]
public class ReturnStringTypedBehavior : TestPipelineBehaviorBase<IReturnStringMessage>;

public class OneReturnStringTypedBehavior : TestPipelineBehaviorBase<OneReturnStringTypedMessage>;

[Priority(Priority.Lowest)]
internal class ReturnStringBehavior : TestPipelineBehaviorBase<IRequest<string>>;

internal class NonDeclaredBehavior : IPipelineBehavior<NonDeclaredMessage, int>
{
    public async Task<int> Handle(NonDeclaredMessage request, RequestHandlerDelegate<int> next, CancellationToken cancellationToken) =>
        await next() + GetType().Name.Length;
}

[Priority(Priority.AboveNormal)]
public class OpenGenericBehavior<TRequest, TResponse>(ILogger<OpenGenericBehavior<TRequest, TResponse>> logger)
    : PipelineBehaviorBase<TRequest, TResponse>()
    where TRequest : IRequest<TResponse>
{
    public override async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (typeof(TResponse) == typeof(string))
        {
            return (TResponse)(object)$"{GetType().Name} > {await next()}";
        }

        logger.LogInformation(GetType().Name);
        return await next();
    }
}
