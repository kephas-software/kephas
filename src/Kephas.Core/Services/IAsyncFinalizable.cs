// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncFinalizable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
        /// <param name="context">Optional. An optional context for finalization.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        Task FinalizeAsync(IContext context = null, CancellationToken cancellationToken = default);
    }
}