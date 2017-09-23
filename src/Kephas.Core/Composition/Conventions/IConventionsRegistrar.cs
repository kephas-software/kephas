// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConventionsRegistrar.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Registrar for composition conventions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Composition.Hosting;

    /// <summary>
    /// Registrar for composition conventions.
    /// </summary>
    public interface IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes, ICompositionRegistrationContext registrationContext);
    }
}