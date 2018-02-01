// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncFinalizable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAsyncFinalizable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides the <see cref="FinalizeAsync"/> method for service asynchronous finalization.
    /// </summary>
    public interface IAsyncFinalizable
    {
        /// <summary>
        /// Finalizes the service.
        /// </summary>
        /// <param name="context">An optional context for finalization.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default);
    }
}