// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Handlers.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.Mediator;
using MediatR;

namespace Kephas.Extensions.Tests.TestServices;

public abstract class TestRequestHandlerBase<TRequest> : RequestHandlerBase<TRequest, string>
    where TRequest : IRequest<string>
{
    public override Task<string> Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(GetType().Name);
}

public class ReturnStringHandler : TestRequestHandlerBase<IRequest<string>>;

[Priority(Priority.High)]
internal class ReturnStringInterfaceHandler : TestRequestHandlerBase<IReturnStringMessage>;

public class OneReturnStringTypedHandler : TestRequestHandlerBase<OneReturnStringTypedMessage>;

public class NonDeclaredHandler : IRequestHandler<NonDeclaredMessage, int>
{
    public Task<int> Handle(NonDeclaredMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(GetType().Name.Length);
}