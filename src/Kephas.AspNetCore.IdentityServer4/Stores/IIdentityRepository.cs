// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdentityRepository.cs" company="Kephas Software SRL">
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

    using Kephas.Services;
    using Microsoft.AspNetCore.Identity;

    /// <summary>
    /// Service for storing items.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IIdentityRepository
    {
        /// <summary>
        /// Gets the result of executing a query transformation over a certain type.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="transformation">The query transformation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The result of executing the query transformation.</returns>
        Task<TResult> QueryAsync<T, TResult>(
            Func<IIdentityRepository, IQueryable<T>, CancellationToken, Task<TResult>> transformation,
            CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new item in a store as an asynchronous operation.
        /// </summary>
        /// <param name="item">The item to create in the store.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that represents the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the asynchronous query.</returns>
        Task<IdentityResult> CreateAsync<T>(T item, string id, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the specified <paramref name="item" /> in the store.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        Task<IdentityResult> UpdateAsync<T>(T item, string id, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the specified <paramref name="item" /> from the store.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The <see cref="T:System.Threading.CancellationToken" /> used to propagate notifications that the operation should be canceled.</param>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="T:Microsoft.AspNetCore.Identity.IdentityResult" /> of the update operation.</returns>
        Task<IdentityResult> DeleteAsync<T>(T item, string id, CancellationToken cancellationToken);

        /// <summary>
        /// Tries to find the item by identifier.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<T?> FindByIdAsync<T>(string id, CancellationToken cancellationToken);
    }
}