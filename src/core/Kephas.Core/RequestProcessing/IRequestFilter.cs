// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestFilter.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for request processing interception.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using Kephas.Services;

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    public interface IRequestFilter
    {
    }

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    [AppServiceContract(AppServiceLifetime.Instance, AllowMultiple = true, MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) })]
    public interface IRequestFilter<TRequest, TResponse> : IRequestFilter
        where TRequest : IRequest
        where TResponse : IResponse
    {
    }
}