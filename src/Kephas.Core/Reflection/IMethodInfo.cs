// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for method information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;

    /// <summary>
    /// Contract for method information.
    /// </summary>
    [ContractClass(typeof(MethodInfoContractClass))]
    public interface IMethodInfo : IElementInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo ReturnType { get; }
    }

    /// <summary>
    /// Contract class for <see cref="IMethodInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IMethodInfo))]
    internal abstract class MethodInfoContractClass : IMethodInfo
    {
        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public abstract string FullName { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public abstract IEnumerable<object> Annotations { get; }

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer { get; }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public ITypeInfo ReturnType
        {
            get
            {
                Contract.Ensures(Contract.Result<ITypeInfo>() != null);

                return Contract.Result<ITypeInfo>();
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
        /// The <see cref="object" /> identified by the key.
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