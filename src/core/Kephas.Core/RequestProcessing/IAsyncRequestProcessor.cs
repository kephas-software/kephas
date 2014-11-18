// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for asynchronous request processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for asynchronous request processors.
    /// </summary>
    public interface IAsyncRequestProcessor
    {
        /// <summary>
        /// Processes the specified request asynchronously.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The response promise.</returns>
        Task<IResponse> ProcessAsync(IRequest request);
    }
}