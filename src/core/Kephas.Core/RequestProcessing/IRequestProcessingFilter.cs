// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessingFilter.cs" company="Quartz Software SRL">
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
    public interface IRequestProcessingFilter
    {
    }

    /// <summary>
    /// Application service for request processing interception.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    [AppServiceContract(AppServiceLifetime.Instance, AllowMultiple = true, 
        MetadataAttributes = new[] { typeof(ProcessingPriorityAttribute) }, 
        ContractType = typeof(IRequestProcessingFilter))]
    public interface IRequestProcessingFilter<TRequest> : IRequestProcessingFilter
        where TRequest : IRequest
    {
    }
}