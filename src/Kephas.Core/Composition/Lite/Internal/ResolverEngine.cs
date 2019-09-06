// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the resolver engine class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Lite.Internal
{
    using System;
    using Kephas;

    /// <summary>
    /// A resolver engine.
    /// </summary>
    internal class ResolverEngine : IResolverEngine
    {
        private readonly WeakReference<IAmbientServices> ambientServicesRef;

        private readonly IServiceRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolverEngine"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="registry">The registry.</param>
        public ResolverEngine(IAmbientServices ambientServices, IServiceRegistry registry)
        {
            this.ambientServicesRef = new WeakReference<IAmbientServices>(ambientServices);
            this.registry = registry;
        }

        public object GetService(Type serviceType)
        {
            if (!this.ambientServicesRef.TryGetTarget(out var ambientServices))
            {
                throw new ObjectDisposedException(nameof(ResolverEngine));
            }

            // simple registration
            if (this.registry.TryGet(serviceType, out var serviceRegistration))
            {
                return serviceRegistration.GetService(ambientServices);
            }

            // open generic registration
            if (serviceType.IsConstructedGenericType)
            {
                var openServiceType = serviceType.GetGenericTypeDefinition();

                if (this.registry.TryGet(openServiceType, out serviceRegistration))
                {
                    serviceRegistration = this.registry.GetOrRegister(serviceType, _ => serviceRegistration.MakeGenericServiceInfo(ambientServices, serviceType.GetGenericArguments()));
                    return serviceRegistration.GetService(ambientServices);
                }
            }

            // source registration
            foreach (var source in this.registry.Sources)
            {
                if (source.IsMatch(serviceType))
                {
                    return source.GetService(ambientServices, serviceType);
                }
            }

            return null;
        }
    }
}