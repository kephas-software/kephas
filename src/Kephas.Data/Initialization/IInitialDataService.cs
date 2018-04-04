// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitialDataService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInitialDataService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Application service contract for creating initial data.
    /// </summary>
    /// <remarks>
    /// The typical implementation aggregates multiple <see cref="IInitialDataHandler"/> services
    /// and calls them in their priority order.
    /// </remarks>
    [SharedAppServiceContract]
    public interface IInitialDataService
    {
        /// <summary>
        /// Creates the initial data asynchronously.
        /// </summary>
        /// <param name="initialDataContext">Context for the initial data.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// An asynchronous result returning the data creation result.
        /// </returns>
        Task<object> CreateDataAsync(
            IInitialDataContext initialDataContext,
            CancellationToken cancellationToken = default);        
    }
}