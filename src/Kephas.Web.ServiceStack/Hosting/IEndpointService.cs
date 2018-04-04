// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpointService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEndpointService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting
{
    using Kephas.Application;
    using Kephas.Services;

    /// <summary>
    /// Endpoint application service.
    /// </summary>
    [AppServiceContract(AllowMultiple = true, MetadataAttributes = new[] { typeof(RequiredFeatureAttribute) })]
    public interface IEndpointService
    {
    }
}