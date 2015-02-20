// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides an indexer for getting and setting custom values.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Provides an indexer for getting and setting custom values.
    /// </summary>
    [ContractClass(typeof(ContextContractClass))]
    public interface IContext
    {
        /// <summary>
        /// Gets the custom values.
        /// </summary>
        /// <value>
        /// The custom values.
        /// </value>
        dynamic Data { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IContext"/>.
    /// </summary>
    [ContractClassFor(typeof(IContext))]
    internal abstract class ContextContractClass : IContext
    {
        /// <summary>
        /// Gets the custom values.
        /// </summary>
        /// <value>
        /// The custom values.
        /// </value>
        public dynamic Data
        {
            get
            {
                Contract.Ensures((object)Contract.Result<dynamic>() != null);
                return Contract.Result<dynamic>();
            }
        }
    }
}