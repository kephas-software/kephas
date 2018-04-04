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
        /// Initializes a new instance of the <see cref="ServiceCollectionConventionsRegistrar"/>
        /// class.
        /// </summary>
        /// <param name="services">The services.</param>
        public ServiceCollectionConventionsRegistrar(IServiceCollection services)
        {
            
        }

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
            // TODO get the services collection from the registation context expando
            // put there by the WebStartup
            throw new System.NotImplementedException();
        }
    }
}