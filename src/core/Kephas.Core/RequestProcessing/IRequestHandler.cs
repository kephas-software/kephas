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
    using Kephas.Services;

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    public interface IRequestHandler
    {
    }

    /// <summary>
    /// Application service for handling requests.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    [AppServiceContract(AppServiceLifetime.Instance)]
    public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
    {
    }
}