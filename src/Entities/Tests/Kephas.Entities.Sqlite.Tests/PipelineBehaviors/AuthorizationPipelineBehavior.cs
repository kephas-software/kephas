// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Tests.Security;
using Kephas.Threading;
using MediatR;

namespace Kephas.Entities.Tests.PipelineBehaviors;

public class AuthorizationPipelineBehavior<TRequest, TResponse>(IAsyncLocalContext asyncLocalContext)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = asyncLocalContext.GetValue<UserInfo>();
        
        throw new NotImplementedException();
    }
}