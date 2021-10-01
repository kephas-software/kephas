// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolverEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the resolver engine class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Kephas.Injection.Lite.Internal
{
    /// <summary>
    /// A resolver engine.
    /// </summary>
    internal class ResolverEngine : IResolverEngine
    {
        private readonly WeakReference<IServiceProvider> ambientServicesRef;

        private readonly IServiceRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResolverEngine"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="registry">The serviceRegistry.</param>
        public ResolverEngine(IServiceProvider ambientServices, IServiceRegistry registry)
        {
            this.ambientServicesRef = new WeakReference<IServiceProvider>(ambientServices);
            this.registry = registry;
        }

        public object? GetService(Type contractType)
        {
            if (!this.ambientServicesRef.TryGetTarget(out var ambientServices))
            {
                throw new ObjectDisposedException(nameof(ResolverEngine));
            }

            // simple registration
            if (this.registry.TryGet(contractType, out var serviceRegistration))
            {
                return serviceRegistration.GetService(ambientServices);
            }

            // open generic registration
            if (contractType.IsConstructedGenericType)
            {
                var openServiceType = contractType.GetGenericTypeDefinition();

                if (this.registry.TryGet(openServiceType, out serviceRegistration))
                {
                    serviceRegistration = this.registry.GetOrRegister(contractType, _ => serviceRegistration.MakeGenericServiceInfo(ambientServices, contractType.GetGenericArguments()));
                    return serviceRegistration.GetService(ambientServices);
                }
            }

            // source registration
            foreach (var source in this.registry.Sources)
            {
                if (source.IsMatch(contractType))
                {
                    return source.GetService(ambientServices, contractType);
                }
            }

            return null;
        }
    }
}