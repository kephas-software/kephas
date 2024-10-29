// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Handlers.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.Mediator;
using MediatR;

namespace Kephas.Extensions.Tests.TestServices;

public abstract class TestStreamRequestHandlerBase<TRequest> : StreamRequestHandlerBase<TRequest, string>
    where TRequest : IStreamRequest<string>
{
    public override async IAsyncEnumerable<string> Handle(TRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        yield return GetType().Name;
    }
}

public class StreamReturnStringHandler : TestStreamRequestHandlerBase<IStreamRequest<string>>;

[Priority(Priority.High)]
internal class StreamReturnStringInterfaceHandler : TestStreamRequestHandlerBase<IStreamReturnStringMessage>;

public class OneStreamReturnStringTypedHandler : TestStreamRequestHandlerBase<OneStreamReturnStringTypedMessage>;

public class StreamNonDeclaredHandler : IStreamRequestHandler<StreamNonDeclaredMessage, int>
{
    public async IAsyncEnumerable<int> Handle(StreamNonDeclaredMessage request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Delay(10, cancellationToken);
        yield return GetType().Name.Length;
    }
}