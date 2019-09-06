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

    internal class ResolverEngine : IResolverEngine
    {
        private readonly WeakReference<IAmbientServices> ambientServicesRef;

        private readonly IServiceRegistry registry;

        public ResolverEngine(IAmbientServices ambientServices, IServiceRegistry registry)
        {
            ambientServicesRef = new WeakReference<IAmbientServices>(ambientServices);
            this.registry = registry;
        }

        public object GetService(Type serviceType)
        {
            if (!ambientServicesRef.TryGetTarget(out var ambientServices))
            {
                throw new ObjectDisposedException(nameof(ResolverEngine));
            }

            if (registry.TryGetValue(serviceType, out var serviceRegistration))
            {
                return serviceRegistration.GetService(ambientServices);
            }

            foreach (var source in registry.Sources)
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