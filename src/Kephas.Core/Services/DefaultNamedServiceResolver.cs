// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultNamedServiceResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default named service provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Linq;
    using System.Reflection;

    using Kephas.Injection;
    using Kephas.Resources;

    /// <summary>
    /// A default named service provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultNamedServiceResolver : INamedServiceResolver
    {
        private readonly IInjector injector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNamedServiceResolver"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public DefaultNamedServiceResolver(IInjector injector)
        {
            this.injector = injector;
        }

        /// <summary>
        /// Gets the service with the provided name.
        /// </summary>
        /// <typeparam name="TContract">Type of the service contract.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        /// The named service.
        /// </returns>
        public TContract GetNamedService<TContract>(string serviceName)
        {
            var exportFactories = this.injector
                .GetExportFactories<TContract, AppServiceMetadata>()
                .Order()
                .Where(f => f.Metadata.ServiceName == serviceName)
                .ToList();
            if (exportFactories.Count == 0)
            {
                throw new ServiceException(string.Format(Strings.DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception, serviceName, typeof(TContract)));
            }

            if (exportFactories.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Strings.DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception, serviceName, typeof(TContract)));
            }

            return exportFactories[0].CreateExportedValue();
        }
    }
}