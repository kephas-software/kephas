// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncInitializable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides the <see cref="InitializeServiceAsync" /> method for asynchronous service initialization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the <see cref="InitializeAsync"/> method for asynchronous service initialization.
    /// </summary>
    public interface IAsyncInitializable
    {
        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An awaitable task.
        /// </returns>
        Task InitializeAsync(IContext? context = null, CancellationToken cancellationToken = default);
    }
}