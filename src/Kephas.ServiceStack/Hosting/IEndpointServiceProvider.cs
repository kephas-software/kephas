// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpointServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEndpointServiceProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using System.Reflection;

    using Kephas.Services;

    /// <summary>
    /// The endpoint service provider is a shared application service which collects a list of assemblies
    /// containing the endpoint service classes.
    /// </summary>
    [SharedAppServiceContract]
    public interface IEndpointServiceProvider
    {
        /// <summary>
        /// Gets the list of service assemblies which provide web service implementations for the endpoint.
        /// </summary>
        Assembly[] ServiceAssemblies { get; }

        /// <summary>
        /// Gets the service name.
        /// </summary>
        /// <remarks>
        /// This is simple a concatenated string of all used web endpoints. ServiceStack needs a name for the self host instance.
        /// </remarks>
        string ServiceName { get; }
    }
}