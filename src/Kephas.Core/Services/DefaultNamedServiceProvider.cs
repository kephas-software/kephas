﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultNamedServiceProvider.cs" company="Kephas Software SRL">
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

    using Kephas.Composition;
    using Kephas.Resources;
    using Kephas.Services.Composition;

    /// <summary>
    /// A default named service provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultNamedServiceProvider : INamedServiceProvider
    {
        /// <summary>
        /// Context for the composition.
        /// </summary>
        private readonly ICompositionContext compositionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultNamedServiceProvider"/> class.
        /// </summary>
        /// <param name="compositionContext">Context for the composition.</param>
        public DefaultNamedServiceProvider(ICompositionContext compositionContext)
        {
            this.compositionContext = compositionContext;
        }

        /// <summary>
        /// Gets the service with the provided name.
        /// </summary>
        /// <typeparam name="TService">Type of the service.</typeparam>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>
        /// The named service.
        /// </returns>
        public TService GetNamedService<TService>(string serviceName)
        {
            var exportFactories = this.compositionContext
                .GetExportFactories<TService, AppServiceMetadata>()
                .Order()
                .Where(f => f.Metadata.ServiceName == serviceName)
                .ToList();
            if (exportFactories.Count == 0)
            {
                throw new ServiceException(string.Format(Strings.DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception, serviceName, typeof(TService)));
            }

            if (exportFactories.Count > 1)
            {
                throw new AmbiguousMatchException(string.Format(Strings.DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception, serviceName, typeof(TService)));
            }

            return exportFactories[0].CreateExportedValue();
        }
    }
}