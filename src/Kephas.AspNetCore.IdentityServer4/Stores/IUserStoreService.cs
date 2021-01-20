// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUserStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Transient service contract for user store.
    /// </summary>
    /// <typeparam name="TUser">The type of the user.</typeparam>
    /// <seealso cref="IUserStore{TUser}" />
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IUserStoreService<TUser> : IUserStore<TUser>
        where TUser : class
    {
    }
}
