// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for processing requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using Kephas.Services;

    /// <summary>
    /// Application service for processing requests.
    /// </summary>
    /// <remarks>
    /// The request processor is defined as a shared service.
    /// </remarks>
    [AppServiceContract]
    public interface IRequestProcessor
    {
    }
}