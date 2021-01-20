// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRoleStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Transient service contract for role store.
    /// </summary>
    /// <typeparam name="TRole">The type of the role.</typeparam>
    /// <seealso cref="IRoleStore{TRole}" />
    [AppServiceContract(AsOpenGeneric = true)]
    public interface IRoleStoreService<TRole> : IRoleStore<TRole>
        where TRole : class
    {
    }
}
