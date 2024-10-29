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

public abstract class TestRequestVoidHandlerBase<TRequest> : RequestHandlerBase<TRequest>
    where TRequest : IRequest
{
    public override Task Handle(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(GetType().Name);
}

public class ReturnVoidHandler : TestRequestVoidHandlerBase<IReturnVoidMessageBase>;

[Priority(Priority.High)]
internal class ReturnVoidInterfaceHandler : TestRequestVoidHandlerBase<IReturnVoidMessage>;

public class OneReturnVoidTypedHandler : TestRequestVoidHandlerBase<OneReturnVoidTypedMessage>;

public class NonDeclaredVoidHandler : IRequestHandler<NonDeclaredVoidMessage>
{
    public Task Handle(NonDeclaredVoidMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(GetType().Name.Length);
}