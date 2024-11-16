// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorizationRepositoryInterceptor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Context;
using Kephas.Entities.Interceptors;
using Kephas.Entities.Tests.Security;

namespace Kephas.Entities.Tests.Interceptors;

public class AuthorizationRepositoryInterceptor(IContextAccessor contextAccessor)
    : ISaveChangesRepositoryInterceptor
{
    public async Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next,
        CancellationToken cancellationToken)
    {
        var user = contextAccessor.GetValue<UserInfo>();
        // check permissions
        throw new NotImplementedException();
    }
}