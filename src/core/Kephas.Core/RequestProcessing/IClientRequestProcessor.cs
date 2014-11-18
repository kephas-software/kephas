// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientRequestProcessor.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Application service for clients of request processors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing
{
    using Kephas.Services;

    /// <summary>
    /// Application service for clients of request processors.
    /// </summary>
    /// <remarks>
    /// The client request processor is defined as a shared service.
    /// </remarks>
    [AppServiceContract]
    public interface IClientRequestProcessor : IAsyncRequestProcessor
    {
    }
}