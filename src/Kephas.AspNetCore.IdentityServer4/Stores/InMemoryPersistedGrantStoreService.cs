// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryPersistedGrantStoreService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::IdentityServer4.Extensions;
    using global::IdentityServer4.Models;
    using global::IdentityServer4.Stores;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application service for in-memory persisted grant store.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class InMemoryPersistedGrantStoreService : IPersistedGrantStoreService
    {
        private readonly IInMemoryIdentityRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryPersistedGrantStoreService"/> class.
        /// </summary>
        /// <param name="repository">The in-memory repository.</param>
        public InMemoryPersistedGrantStoreService(IInMemoryIdentityRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>Stores the grant.</summary>
        /// <param name="grant">The grant.</param>
        /// <returns>The asynchronous result.</returns>
        public async Task StoreAsync(PersistedGrant grant)
        {
            var existing = await this.repository.FindByIdAsync<PersistedGrant>(grant.Key, default)
                .PreserveThreadContext();
            if (existing == null)
            {
                await this.repository.CreateAsync(grant, grant.Key, default).PreserveThreadContext();
            }
            else
            {
                await this.repository.UpdateAsync(grant, grant.Key, default).PreserveThreadContext();
            }
        }

        /// <summary>Gets the grant.</summary>
        /// <param name="key">The key.</param>
        /// <returns>An asynchronous result yielding the persisted grant.</returns>
        public Task<PersistedGrant> GetAsync(string key)
            => this.repository.FindByIdAsync<PersistedGrant>(key, default);

        /// <summary>Gets all grants based on the filter.</summary>
        /// <param name="filter">The filter.</param>
        /// <returns>An asynchronous result yielding an enumeration of persisted grants.</returns>
        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var items = this.Filter(filter);

            return Task.FromResult(items);
        }

        /// <summary>Removes the grant by key.</summary>
        /// <param name="key">The key.</param>
        /// <returns>An asynchronous result.</returns>
        public async Task RemoveAsync(string key)
            => await this.repository.DeleteAsync<PersistedGrant>(
                await this.repository.FindByIdAsync<PersistedGrant>(key, default).PreserveThreadContext(),
                key,
                default).PreserveThreadContext();

        /// <summary>Removes all grants based on the filter.</summary>
        /// <param name="filter">The filter.</param>
        /// <returns>An asynchronous result.</returns>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var items = this.Filter(filter);

            foreach (var item in items)
            {
                await this.repository.DeleteAsync(item, item.Key, default).PreserveThreadContext();
            }
        }

        private IEnumerable<PersistedGrant> Filter(PersistedGrantFilter filter)
        {
            var query = this.repository.Query<PersistedGrant>();

            if (!string.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query.Where(x => x.SessionId == filter.SessionId);
            }

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }

            if (!string.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            var items = query.ToArray().AsEnumerable();
            return items;
        }
    }
}