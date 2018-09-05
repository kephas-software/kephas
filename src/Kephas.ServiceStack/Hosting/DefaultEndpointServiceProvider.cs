// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultEndpointServiceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default endpoint service provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Hosting
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Services;
    using Kephas.ServiceStack.Hosting.Composition;

    /// <summary>
    /// A default endpoint service provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultEndpointServiceProvider : IEndpointServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultEndpointServiceProvider" /> class.
        /// </summary>
        /// <param name="endpointServiceFactories">The endpoint service factories.</param>
        public DefaultEndpointServiceProvider(ICollection<IExportFactory<IEndpointService, EndpointMetadata>> endpointServiceFactories)
        {
            var myMetadata = (from s in endpointServiceFactories
                              where s.Metadata.RequiredFeature != null
                              select s.Metadata).ToArray();
            var requiredFeatures = myMetadata.Select(a => a.RequiredFeature).Distinct().ToArray();
            this.ServiceAssemblies = myMetadata.Select(s => s.AppServiceImplementationType.Assembly).Distinct().ToArray();
            this.ServiceName = string.Join("_", requiredFeatures);
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the endpoint service assemblies.
        /// </summary>
        /// <value>
        /// An array of assemblies containing endpoint services.
        /// </value>
        public Assembly[] ServiceAssemblies { get; }
    }
}