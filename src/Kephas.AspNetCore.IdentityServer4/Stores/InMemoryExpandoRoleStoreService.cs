// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryExpandoRoleStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Composition.AttributedModel;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// An in-memory service for storing <see cref="Expando"/> roles.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InMemoryExpandoRoleStoreService : InMemoryExpandoRoleStoreService<Expando>
    {
    }

    /// <summary>
    /// An in-memory service for storing <see cref="IExpando"/> based roles.
    /// </summary>
    /// <typeparam name="TRole">The role type.</typeparam>
    [ExcludeFromComposition]
    public class InMemoryExpandoRoleStoreService<TRole> : ExpandoRoleStoreServiceBase<TRole>
        where TRole : class, IExpando
    {
        private readonly ConcurrentDictionary<string, TRole> rolesById = new ConcurrentDictionary<string, TRole>();

        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            var id = this.GetRoleId(role);
            return Task.FromResult(this.rolesById.TryAdd(id, role)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"Role with ID '{id}' already added." }));
        }

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            var id = this.GetRoleId(role);
            return Task.FromResult(this.rolesById.TryUpdate(id, role, role)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"Role with ID '{id}' not found." }));
        }

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            var id = this.GetRoleId(role);
            return Task.FromResult(this.rolesById.TryRemove(id, out _)
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError { Description = $"Role with ID '{id}' not found." }));
        }

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public override Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            var role = this.rolesById.TryGetValue(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID '{roleId}' not found.");
            }

            return Task.FromResult(role);
        }

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public override Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var role = this.rolesById.Values.FirstOrDefault(u => normalizedRoleName.Equals(this.GetRoleName(u), StringComparison.OrdinalIgnoreCase));
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with name '{normalizedRoleName}' not found.");
            }

            return Task.FromResult(role);
        }
    }
}