// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the service collection conventions registrar class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Services.Composition
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;

    /// <summary>
    /// A conventions registrar for a collection of service definitions.
    /// </summary>
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
            // TODO get the services collection from the registation context expando
            // put there by the WebStartup
            throw new System.NotImplementedException();
        }
    }
}