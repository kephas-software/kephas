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
    using System.Diagnostics.Contracts;
    using System.Reflection;

    /// <summary>
    /// Registrar for composition conventions.
    /// </summary>
    [ContractClass(typeof(ConventionsRegistrarContractClass))]
    public interface IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes);
    }

    /// <summary>
    /// Contract class for <see cref="IConventionsRegistrar"/>.
    /// </summary>
    [ContractClassFor(typeof(IConventionsRegistrar))]
    internal abstract class ConventionsRegistrarContractClass : IConventionsRegistrar
    {
        /// <summary>
        /// Registers the conventions.
        /// </summary>
        /// <param name="builder">The registration builder.</param>
        /// <param name="candidateTypes">The candidate types which can take part in the composition.</param>
        public void RegisterConventions(IConventionsBuilder builder, IEnumerable<TypeInfo> candidateTypes)
        {
            Contract.Requires(builder != null);
            Contract.Requires(candidateTypes != null);
        }
    }
}