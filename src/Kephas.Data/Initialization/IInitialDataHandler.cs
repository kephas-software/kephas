// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInitialDataHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IInitialDataHandler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Initialization.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for handling initial data.
    /// </summary>
    [SharedAppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(InitialDataKindAttribute) })]
    public interface IInitialDataHandler
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