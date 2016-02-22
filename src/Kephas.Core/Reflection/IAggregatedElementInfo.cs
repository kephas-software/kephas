// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAggregatedElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for aggregated element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;

    /// <summary>
    /// Contract for aggregated element information.
    /// </summary>
    [ContractClass(typeof(AggregatedElementInfoContractClass))]
    public interface IAggregatedElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        IEnumerable<IElementInfo> Parts { get; }  
    }

    /// <summary>
    /// Contract class for <see cref="IAggregatedElementInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IAggregatedElementInfo))]
    internal abstract class AggregatedElementInfoContractClass : IAggregatedElementInfo
    {
        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public abstract IEnumerable<object> Annotations { get; }

        /// <summary>
        /// Gets the parts of an aggregated element.
        /// </summary>
        /// <value>
        /// The parts.
        /// </value>
        public IEnumerable<IElementInfo> Parts
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<IElementInfo>>() != null);

                return Contract.Result<IEnumerable<IElementInfo>>();
            }
        }

        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Properties collection AND the strongly typed
        /// properties of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Returns the <see cref="T:System.Dynamic.DynamicMetaObject"/> responsible for binding operations performed on this object.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Dynamic.DynamicMetaObject"/> to bind this object.
        /// </returns>
        /// <param name="parameter">The expression tree representation of the runtime value.</param>
        public abstract DynamicMetaObject GetMetaObject(Expression parameter);
    }
}