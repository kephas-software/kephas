// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Entities.Interceptors;
using Kephas.Entities.Tests.Security;
using Kephas.Threading;

namespace Kephas.Entities.Tests.Interceptors;

public class AuthorizationRepositoryInterceptor(IAsyncLocalContext asyncLocalContext)
    : ISaveChangesRepositoryInterceptor
{
    public Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next,
        CancellationToken cancellationToken)
    {
        // check whether the user is authorized to perform the operation
        // for test purposes check only whether the user is authenticated 
        var user = asyncLocalContext.TryGetValue<UserInfo>();
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        return next();
    }
}