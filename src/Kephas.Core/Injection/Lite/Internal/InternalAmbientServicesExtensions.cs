// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InternalAmbientServicesExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the internal ambient services extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Injection.Lite.Internal
{
    using System;
    using Kephas.Dynamic;

    internal static class InternalAmbientServicesExtensions
    {
        internal static IInjector AsInjector(this IServiceProvider serviceProvider)
        {
            if (serviceProvider is not IDynamic dynamicServiceProvider)
            {
                return serviceProvider.ToInjector();
            }

            const string AsInjectorKey = "__AsInjector";
            if (dynamicServiceProvider[AsInjectorKey] is IInjector injector)
            {
                return injector;
            }

            injector = serviceProvider.ToInjector();
            dynamicServiceProvider[AsInjectorKey] = injector;
            return injector;
        }

        internal static TMetadata CreateTypedMetadata<TMetadata>(this IServiceInfo serviceInfo)
        {
            return (TMetadata)Activator.CreateInstance(typeof(TMetadata), serviceInfo.Metadata);
        }

        /// <summary>
        /// Gets a value indicating whether the service with the provided contract is registered.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>
        /// <c>true</c> if the service is registered, <c>false</c> if not.
        /// </returns>
        internal static bool IsRegistered(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider is IAmbientServices ambientServices)
            {
                return ambientServices.IsRegistered(serviceType);
            }

            return false;
        }
    }
}