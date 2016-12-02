// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAmbientServicesAware.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAmbientServicesAware interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Interface for components being aware of the ambient services within they live.
    /// </summary>
    [ContractClass(typeof(AmbientServicesAwareContractClass))]
    public interface IAmbientServicesAware
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        IAmbientServices AmbientServices { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IAmbientServicesAware"/>.
    /// </summary>
    [ContractClassFor(typeof(IAmbientServicesAware))]
    internal abstract class AmbientServicesAwareContractClass : IAmbientServicesAware
    {
        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices
        {
            get
            {
                Contract.Ensures(Contract.Result<IAmbientServices>() != null);
                return Contract.Result<IAmbientServices>();
            }
        }
    }
}