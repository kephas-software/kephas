// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicPropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for dynamically accessing a property.
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
    /// Contract for a dynamic <see cref="PropertyInfo"/>.
    /// </summary>
    [ContractClass(typeof(DynamicPropertyInfoContractClass))]
    public interface IDynamicPropertyInfo : IPropertyInfo, IDynamicElementInfo
    {
        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        void SetValue(object obj, object value);

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        object GetValue(object obj);
    }

    /// <summary>
    /// Contract class for <see cref="IDynamicPropertyInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IDynamicPropertyInfo))]
    internal abstract class DynamicPropertyInfoContractClass : IDynamicPropertyInfo
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
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo
        {
            get
            {
                Contract.Ensures(Contract.Result<PropertyInfo>() != null);

                return Contract.Result<PropertyInfo>();
            }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        public abstract ITypeInfo PropertyType { get; }

        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        public abstract bool CanRead { get; }

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
        /// Sets the specified value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public abstract void SetValue(object obj, object value);

        /// <summary>
        /// Gets the value from the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The value.
        /// </returns>
        public abstract object GetValue(object obj);

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
