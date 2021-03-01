﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistedGrantStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using global::IdentityServer4.Stores;
    using Kephas.Services;

    /// <summary>
    /// Transient service contract for persisted grants store.
    /// </summary>
    /// <seealso cref="IPersistedGrantStore" />
    [AppServiceContract]
    public interface IPersistedGrantStoreService : IPersistedGrantStore
    {
    }
}