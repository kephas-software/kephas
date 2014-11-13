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
    using System.Diagnostics.Contracts;

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
        void RegisterConventions(IConventionsBuilder builder);
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
        public void RegisterConventions(IConventionsBuilder builder)
        {
            Contract.Requires(builder != null);
        }
    }
}