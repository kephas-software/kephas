// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeDynamicType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides optimized access to methods and properties at runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides optimized access to methods and properties at runtime.
    /// </summary>
    public class RuntimeDynamicType : IDynamicType
    {
        /// <summary>
        /// The cache of dynamic types.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, RuntimeDynamicType> DynamicTypesCache = new ConcurrentDictionary<Type, RuntimeDynamicType>();

        /// <summary>
        /// The runtime dynamic property generic type information.
        /// </summary>
        private static readonly TypeInfo RuntimeDynamicPropertyGenericTypeInfo = typeof(RuntimeDynamicProperty<,>).GetTypeInfo();

        /// <summary>
        /// The dynamic properties.
        /// </summary>
        private readonly IDictionary<string, IDynamicProperty> dynamicProperties;

        /// <summary>
        /// The dynamic methods.
        /// </summary>
        private readonly IDictionary<string, IDynamicMethod> dynamicMethods;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeDynamicType"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal RuntimeDynamicType(Type type)
        {
            Contract.Requires(type != null);

            this.Type = type;
            this.dynamicProperties = CreateDynamicProperties(type);
            this.dynamicMethods = CreateDynamicMethods(type);
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the dynamic properties.
        /// </summary>
        /// <value>
        /// The dynamic properties.
        /// </value>
        public IEnumerable<KeyValuePair<string, IDynamicProperty>> DynamicProperties
        {
            get { return this.dynamicProperties; }
        }

        /// <summary>
        /// Gets the dynamic methods.
        /// </summary>
        /// <value>
        /// The dynamic methods.
        /// </value>
        public IEnumerable<KeyValuePair<string, IDynamicMethod>> DynamicMethods
        {
            get { return this.dynamicMethods; }
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
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        public object Get(object instance, string propertyName)
        {
            if (instance == null)
            {
                return Undefined.Value;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName);
            return dynamicProperty.Get(instance);
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
        /// If a property with the provided name is not found, the <see cref="Undefined.Value" /> is returned.
        /// </remarks>
        public object TryGet(object instance, string propertyName)
        {
            if (instance == null)
            {
                return Undefined.Value;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName, throwOnNotFound: false);
            return dynamicProperty == null ? Undefined.Value : dynamicProperty.Get(instance);
        }

        /// <summary>
        /// Sets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <remarks>
        /// If a property with the provided name is not found, an exception occurs.
        /// </remarks>
        public void Set(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                return;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName);
            dynamicProperty.Set(instance, value);
        }

        /// <summary>
        /// Tries to set the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the value could be set; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If a property with the provided name is not found, the method just returns.
        /// </remarks>
        public bool TrySet(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                return false;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName, throwOnNotFound: false);
            if (dynamicProperty != null)
            {
                dynamicProperty.Set(instance, value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The invocation result.
        /// </returns>
        public object Invoke(object instance, string methodName, IEnumerable<object> args)
        {
            if (instance == null)
            {
                return null;
            }

            var dynamicMethod = this.GetDynamicMethod(methodName);
            return dynamicMethod.Invoke(instance, args);
        }

        /// <summary>
        /// Tries to invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The invocation result, if the method exists, otherwise <see cref="Undefined.Value" />.
        /// </returns>
        public object TryInvoke(object instance, string methodName, IEnumerable<object> args)
        {
            if (instance == null)
            {
                return Undefined.Value;
            }

            var dynamicMethod = this.GetDynamicMethod(methodName, throwOnNotFound: false);
            if (dynamicMethod != null)
            {
                return dynamicMethod.TryInvoke(instance, args);
            }

            return Undefined.Value;
        }

        /// <summary>
        /// Gets the dynamic type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime dynamic type.</returns>
        internal static RuntimeDynamicType GetDynamicType(Type type)
        {
            Contract.Requires(type != null);

            return DynamicTypesCache.GetOrAdd(type, _ => new RuntimeDynamicType(type));
        }

        /// <summary>
        /// Creates the dynamic methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A dictionary of runtime dynamic methods.</returns>
        private static IDictionary<string, IDynamicMethod> CreateDynamicMethods(Type type)
        {
            var methodInfos = type.GetRuntimeMethods();
            return methodInfos.ToDictionary(mi => mi.Name, mi => (IDynamicMethod)new RuntimeDynamicMethod(mi));
        }

        /// <summary>
        /// Creates the dynamic properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A dictionary of dynamic properties.
        /// </returns>
        private static IDictionary<string, IDynamicProperty> CreateDynamicProperties(Type type)
        {
            var dynamicProperties = new Dictionary<string, IDynamicProperty>();
            var propertyInfos = type.GetRuntimeProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                if (propertyInfo.GetIndexParameters().Length > 0 || dynamicProperties.ContainsKey(propertyName))
                {
                    continue;
                }

                var runtimePropertyAccessorType = RuntimeDynamicPropertyGenericTypeInfo.MakeGenericType(
                    type,
                    propertyInfo.PropertyType);
                var constructor = runtimePropertyAccessorType.GetTypeInfo().DeclaredConstructors.First();
                var propertyAccessor = (IDynamicProperty)constructor.Invoke(new object[] { propertyInfo });
                dynamicProperties.Add(propertyName, propertyAccessor);
            }

            return dynamicProperties;
        }

        /// <summary>
        /// Gets the dynamic property for the provided property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the property is not found.</param>
        /// <returns>
        /// The dynamic property.
        /// </returns>
        private IDynamicProperty GetDynamicProperty(string propertyName, bool throwOnNotFound = true)
        {
            IDynamicProperty dynamicProperty;
            if (!this.dynamicProperties.TryGetValue(propertyName, out dynamicProperty))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException(string.Format("Property {0} not found or is not accessible in {1}.", propertyName, this.Type));
                }

                return null;
            }

            return dynamicProperty;
        }

        /// <summary>
        /// Gets the dynamic method for the provided property name.
        /// </summary>
        /// <param name="methodName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the property is not found.</param>
        /// <returns>
        /// The dynamic method.
        /// </returns>
        private IDynamicMethod GetDynamicMethod(string methodName, bool throwOnNotFound = true)
        {
            IDynamicMethod dynamicMethod;
            if (!this.dynamicMethods.TryGetValue(methodName, out dynamicMethod))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException(string.Format("Method {0} not found or is not accessible in {1}.", methodName, this.Type));
                }

                return null;
            }

            return dynamicMethod;
        }
    }
}
