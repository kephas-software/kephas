// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataBehaviorProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataBehaviorProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Application service contract for providing data behaviors.
    /// </summary>
    [ContractClass(typeof(DataBehaviorProviderContractClass))]
    [SharedAppServiceContract]
    public interface IDataBehaviorProvider
    {
        /// <summary>
        /// Gets the data behaviors of type <typeparamref name="TBehavior"/> for the provided type.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An enumeration of behaviors mathing the provided type.
        /// </returns>
        IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(Type type);
    }

    /// <summary>
    /// A data behavior provider contract class.
    /// </summary>
    [ContractClassFor(typeof(IDataBehaviorProvider))]
    internal abstract class DataBehaviorProviderContractClass : IDataBehaviorProvider
    {
        /// <summary>
        /// Gets the data behaviors of type <typeparamref name="TBehavior"/>
        ///  for the provided type.
        /// </summary>
        /// <typeparam name="TBehavior">Type of the behavior.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>
        /// An enumeration of behaviors mathing the provided type.
        /// </returns>
        public IEnumerable<TBehavior> GetDataBehaviors<TBehavior>(Type type)
        {
            Requires.NotNull(type, nameof(type));
            return Contract.Result<IEnumerable<TBehavior>>();
        }
    }
}