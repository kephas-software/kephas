// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncInitializable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        Task InitializeAsync(IContext context = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}