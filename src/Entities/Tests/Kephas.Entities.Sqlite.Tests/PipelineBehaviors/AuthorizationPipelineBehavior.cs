// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationPipelineBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Context;
using Kephas.Entities.Tests.Security;
using MediatR;

namespace Kephas.Entities.Tests.PipelineBehaviors;

public class AuthorizationPipelineBehavior<TRequest, TResponse>(IContextAccessor contextAccessor)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var user = contextAccessor.GetValue<UserInfo>();
        
        throw new NotImplementedException();
    }
}