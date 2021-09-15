// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityRoleStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Injection.AttributedModel;
    using Kephas.Logging;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// A service for storing <see cref="IdentityRole"/> instances.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class IdentityRoleStoreService : IdentityRoleStoreService<IdentityRole>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleStoreService"/> class.
        /// </summary>
        /// <param name="repository">The identity repository.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public IdentityRoleStoreService(IIdentityRepository repository, ILogManager? logManager = null)
            : base(repository, logManager)
        {
        }
    }

    /// <summary>
    /// A service for storing IdentityRole based roles.
    /// </summary>
    /// <typeparam name="TRole">The role type.</typeparam>
    [ExcludeFromInjection]
    public class IdentityRoleStoreService<TRole> : IdentityRoleStoreServiceBase<TRole>
        where TRole : IdentityRole
    {
        private readonly IIdentityRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityRoleStoreService{TRole}"/> class.
        /// </summary>
        /// <param name="repository">The identity repository.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public IdentityRoleStoreService(IIdentityRepository repository, ILogManager? logManager = null)
            : base(logManager)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates a new role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to create in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
            => this.repository.CreateAsync(role, role.Id, cancellationToken);

        /// <summary>
        /// Updates a role in a store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to update in the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
            => this.repository.UpdateAsync(role, role.Id, cancellationToken);

        /// <summary>
        /// Deletes a role from the store as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role to delete from the store.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        public override Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
            => this.repository.DeleteAsync(role, role.Id, cancellationToken);

        /// <summary>
        /// Finds the role who has the specified ID as an asynchronous operation.
        /// </summary>
        /// <param name="roleId">The role ID to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public override Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
            => this.repository.FindByIdAsync<TRole>(roleId, cancellationToken);

        /// <summary>
        /// Finds the role who has the specified normalized name as an asynchronous operation.
        /// </summary>
        /// <param name="normalizedRoleName">The normalized role name to look for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that result of the look up.</returns>
        public override Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
            => this.repository.QueryAsync<TRole, TRole>(
                (r, q, ct) =>
                Task.FromResult(q.FirstOrDefault(u => normalizedRoleName.Equals(u.NormalizedName, StringComparison.OrdinalIgnoreCase))),
                default);
    }
}