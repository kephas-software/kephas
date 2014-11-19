// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestHandler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for handling requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    public interface IRequestHandler : IDisposable
    {
        /// <summary>
        /// Processes the provided request and returns the response.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <returns>The response.</returns>
        IResponse Process(IRequest request);
    }

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract(AppServiceLifetime.Instance)]
    public interface IRequestHandler<in TRequest> : IRequestHandler
        where TRequest : IRequest
    {
        /// <summary>
        /// Processes the provided request and returns the response.
        /// </summary>
        /// <param name="request">The request to be handled.</param>
        /// <returns>The response.</returns>
        IResponse Process(TRequest request);
    }
}