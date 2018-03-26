// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEndpointServiceProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEndpointServiceProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting
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