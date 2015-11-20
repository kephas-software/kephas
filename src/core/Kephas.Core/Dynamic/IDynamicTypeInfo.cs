// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicTypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for a dynamic <see cref="TypeInfo" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Dynamic;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="TypeInfo"/>.
    /// </summary>
    [ContractClass(typeof(DynamicTypeInfoContractClass))]
    public interface IDynamicTypeInfo : IExpando
    {
        /// <summary>
        /// Gets the underlying <see cref="Type"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        Type Type { get; }

        /// <summary>
        /// Gets the underlying <see cref="TypeInfo"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        TypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        IEnumerable<KeyValuePair<string, IDynamicPropertyInfo>> Properties { get; }

        /// <summary>
        /// Gets the methods.
        /// </summary>
        /// <value>
        /// The methods.
        /// </value>
        IEnumerable<KeyValuePair<string, IEnumerable<IDynamicMethodInfo>>> Methods { get; }
        
        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        object GetValue(object instance, string propertyName);

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        /// <remarks>
        /// If a property with the provided name is not found, the <see cref="Undefined.Value"/> is returned.
        /// Also, if the object passed is <c>null</c>, then <see cref="Undefined.Value"/> is returned.</remarks>
        object TryGetValue(object instance, string propertyName);

        /// <summary>
        /// Sets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        void SetValue(object instance, string propertyName, object value);

        /// <summary>
        /// Tries to set the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If a property with the provided name is not found, the method just returns.
        /// Also, the method just returns if the instance passed is <c>null</c>.
        /// </remarks>
        bool TrySetValue(object instance, string propertyName, object value);

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        object Invoke(object instance, string methodName, IEnumerable<object> args);

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result, if the method exists, otherwise <see cref="Undefined.Value"/>.</returns>
        object TryInvoke(object instance, string methodName, IEnumerable<object> args);
    }

    /// <summary>
    /// Contract class for <see cref="IDynamicTypeInfo"/>.
    /// </summary>
    [ContractClassFor(typeof(IDynamicTypeInfo))]
    internal abstract class DynamicTypeInfoContractClass : IDynamicTypeInfo
    {
        /// <summary>
        /// Gets the underlying <see cref="IDynamicTypeInfo.Type"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public abstract Type Type { get; }

        /// <summary>
        /// Gets the underlying <see cref="IDynamicTypeInfo.TypeInfo"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public abstract TypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the dynamic properties.
        /// </summary>
        /// <value>
        /// The dynamic properties.
        /// </value>
        public IEnumerable<KeyValuePair<string, IDynamicPropertyInfo>> Properties
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, IDynamicPropertyInfo>>>() != null);
                return Contract.Result<IEnumerable<KeyValuePair<string, IDynamicPropertyInfo>>>();
            }
        }

        /// <summary>
        /// Gets the dynamic methods.
        /// </summary>
        /// <value>
        /// The dynamic methods.
        /// </value>
        public IEnumerable<KeyValuePair<string, IEnumerable<IDynamicMethodInfo>>> Methods
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<KeyValuePair<string, IEnumerable<IDynamicMethodInfo>>>>() != null);
                return Contract.Result<IEnumerable<KeyValuePair<string, IEnumerable<IDynamicMethodInfo>>>>();
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
        /// The <see cref="Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The requested property value.</returns>
        public abstract object this[string key] { get; set; }

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        public object GetValue(object instance, string propertyName)
        {
            Contract.Requires(instance != null);
            return Contract.Result<object>();
        }

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        /// <remarks>
        /// If a property with the provided name is not found, the <see cref="Undefined.Value"/> is returned.
        /// </remarks>
        public abstract object TryGetValue(object instance, string propertyName);

        /// <summary>
        /// Sets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        public void SetValue(object instance, string propertyName, object value)
        {
            Contract.Requires(instance != null);
        }

        /// <summary>
        /// Tries to set the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value could be set; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If a property with the provided name is not found, the method just returns.
        /// </remarks>
        public abstract bool TrySetValue(object instance, string propertyName, object value);

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public object Invoke(object instance, string methodName, IEnumerable<object> args)
        {
            Contract.Requires(instance != null);
            return Contract.Result<object>();
        }

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result, if the method exists, otherwise <see cref="Undefined.Value"/>.</returns>
        public abstract object TryInvoke(object instance, string methodName, IEnumerable<object> args);

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
