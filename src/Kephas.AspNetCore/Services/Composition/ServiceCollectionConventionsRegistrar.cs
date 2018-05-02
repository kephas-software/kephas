// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service collection conventions registrar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Services.Composition
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
    [ExcludeFromComposition]
    public class ServiceCollectionConventionsRegistrar : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(
            IConventionsBuilder builder,
            IEnumerable<TypeInfo> candidateTypes,
            ICompositionRegistrationContext registrationContext)
        {
            var ambientServices = registrationContext.AmbientServices;
            var serviceCollection = ambientServices.GetService<IServiceCollection>();

            foreach (var descriptor in serviceCollection)
            {
                if (descriptor.ImplementationInstance != null)
                {
                    builder.ForInstance(descriptor.ServiceType, descriptor.ImplementationInstance);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    builder.ForInstanceFactory(descriptor.ServiceType, ctx => descriptor.ImplementationFactory((ctx ?? ambientServices.CompositionContainer).ToServiceProvider()));
                }
                else
                {
                    var partBuilder = descriptor.ImplementationType != null
                                          ? builder.ForType(descriptor.ImplementationType)
                                          : builder.ForType(descriptor.ServiceType);

                    if (descriptor.Lifetime == ServiceLifetime.Singleton)
                    {
                        partBuilder.Shared();
                    }
                    else if (descriptor.Lifetime == ServiceLifetime.Scoped)
                    {
                        partBuilder.ScopeShared();
                    }

                    partBuilder.Export(b => b.AsContractType(descriptor.ServiceType));
                }
            }
        }
    }
}