// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMefConventionBuilderProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provider for <see cref="ConventionBuilder" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Mef.Conventions
{
    using System.Composition.Convention;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provider for <see cref="ConventionBuilder"/>.
    /// </summary>
    [ContractClass(typeof(MefConventionBuilderProviderContractClass))]
    public interface IMefConventionBuilderProvider
    {
        /// <summary>
        /// Gets the convention builder.
        /// </summary>
        /// <returns>The convention builder.</returns>
        ConventionBuilder GetConventionBuilder();
    }

    /// <summary>
    /// Contract class for <see cref="IMefConventionBuilderProvider"/>.
    /// </summary>
    [ContractClassFor(typeof(IMefConventionBuilderProvider))]
    internal abstract class MefConventionBuilderProviderContractClass : IMefConventionBuilderProvider
    {
        /// <summary>
        /// The get convention builder.
        /// </summary>
        /// <returns>
        /// The <see cref="ConventionBuilder"/>.
        /// </returns>
        public ConventionBuilder GetConventionBuilder()
        {
            Contract.Ensures(Contract.Result<ConventionBuilder>() != null);
            return new ConventionBuilder();
        }
    }
}