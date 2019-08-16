// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConventionsRegistrar.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Registrar for composition conventions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Conventions
{
    using System;
    using System.Collections.Generic;

    using Kephas.Composition.Hosting;

    /// <summary>
    /// Registrar for composition conventions.
    /// </summary>
    public interface IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <remarks>
        /// The candidate types provided may be changed, typically by adding new types.
        /// </remarks>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        void RegisterConventions(
            IConventionsBuilder builder,
            IList<Type> candidateTypes,
            ICompositionRegistrationContext registrationContext);
    }
}