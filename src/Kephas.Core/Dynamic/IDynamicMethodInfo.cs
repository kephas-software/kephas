// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamically invoking a method.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="MethodInfo"/>.
    /// </summary>
    [ContractClass(typeof(DynamicMethodInfoContractClass))]
    public interface IDynamicMethodInfo : IMethodInfo, IDynamicElementInfo
    {
        /// <summary>
        /// Gets the method information.
        /// </summary>
        /// <value>
        /// The method information.
        /// </value>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object Invoke(object instance, IEnumerable<object> args);

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result, if the method exists, otherwise <see cref="Undefined.Value"/>.</returns>
        object TryInvoke(object instance, IEnumerable<object> args);
    }

    /// <summary>
    /// Contract class for <see cref="IDynamicMethodInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IDynamicMethodInfo))]
    internal abstract class DynamicMethodInfoContractClass : IDynamicMethodInfo
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
        /// Gets the method information.
        /// </summary>
        /// <value>
        /// The method information.
        /// </value>
        public MethodInfo MethodInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<MethodInfo>() != null);

                return Contract.Result<MethodInfo>();
            }
        }

        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        public abstract ITypeInfo ReturnType { get; }

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

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public abstract object Invoke(object instance, IEnumerable<object> args);

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result, if the method exists, otherwise <see cref="Undefined.Value"/>.</returns>
        public abstract object TryInvoke(object instance, IEnumerable<object> args);

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo()
        {
            throw new System.NotImplementedException();
        }
    }
}
