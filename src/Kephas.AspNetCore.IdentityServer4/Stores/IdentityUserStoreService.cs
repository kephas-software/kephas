// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityUserStoreService.cs" company="Kephas Software SRL">
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

    using Kephas.Injection.AttributedModel;
    using Kephas.Logging;
    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// A repository based service for storing <see cref="IdentityUser"/> users.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class IdentityUserStoreService : IdentityUserStoreService<IdentityUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserStoreService"/> class.
        /// </summary>
        /// <param name="repository">The identity repository.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public IdentityUserStoreService(IIdentityRepository repository, ILogManager? logManager = null)
            : base(repository, logManager)
        {
        }
    }

    /// <summary>
    /// A repository based service for storing <see cref="IdentityUser"/> based users.
    /// </summary>
    /// <typeparam name="TUser">The user type.</typeparam>
    [ExcludeFromInjection]
    public class IdentityUserStoreService<TUser> : IdentityUserStoreServiceBase<TUser>
        where TUser : IdentityUser
    {
        private readonly IIdentityRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityUserStoreService{TUser}"/> class.
        /// </summary>
        /// <param name="repository">The identity repository.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public IdentityUserStoreService(IIdentityRepository repository, ILogManager? logManager = null)
            : base(logManager)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the creation operation.</returns>
        public override Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
            => this.repository.CreateAsync<TUser>(user, user.Id, cancellationToken);

        /// <summary>
        /// Updates the specified <paramref name="user" /> in the user store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public override Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
            => this.repository.UpdateAsync(user, user.Id, cancellationToken);

        /// <summary>
        /// Deletes the specified <paramref name="user" /> from the user store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        public override Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
            => this.repository.DeleteAsync<TUser>(user, user.Id, cancellationToken);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId" />.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId" /> if it exists.
        /// </returns>
        public override Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
            => this.repository.FindByIdAsync<TUser>(userId, cancellationToken);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName" /> if it exists.
        /// </returns>
        public override Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
            => this.repository.QueryAsync<TUser, TUser>(
                    (r, q, ct) =>
                        Task.FromResult(q.FirstOrDefault(u => normalizedUserName.Equals(u.NormalizedUserName, StringComparison.OrdinalIgnoreCase))),
                    default);

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public override Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
            => this.repository.QueryAsync<TUser, TUser>(
                    (r, q, ct) =>
                        Task.FromResult(q.FirstOrDefault(u => normalizedEmail.Equals(u.NormalizedEmail, StringComparison.OrdinalIgnoreCase))),
                    default);
    }
}