// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefConventionsRegistrarBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base class for MEF conventions registrars.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Convention;
    using System.Reflection;

    using Kephas.Composition.Conventions;
    using Kephas.Composition.Mef.Resources;
    using Kephas.Services;

    /// <summary>
    /// Base class for MEF conventions registrars.
    /// </summary>
    public abstract class MefConventionsRegistrarBase : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the provided conventions are not MEF conventions.</exception>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        /// <param name="registrationContext">Context for the registration.</param>
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes, IContext registrationContext)
        {
            var mefBuilder = builder as IMefConventionBuilderProvider;
            if (mefBuilder == null)
            {
                throw new InvalidOperationException(string.Format(Strings.InvalidConventions, typeof(IMefConventionBuilderProvider)));
            }

            this.RegisterConventions(mefBuilder.GetConventionBuilder());
        }

        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        protected abstract void RegisterConventions(ConventionBuilder builder);
    }
}