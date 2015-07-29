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
        private readonly IDictionary<string, IList<IDynamicMethod>> dynamicMethods;

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
        public Type Type { get; }

        /// <summary>
        /// Gets the dynamic properties.
        /// </summary>
        /// <value>
        /// The dynamic properties.
        /// </value>
        public IEnumerable<KeyValuePair<string, IDynamicProperty>> DynamicProperties => this.dynamicProperties;

        /// <summary>
        /// Gets the dynamic methods.
        /// </summary>
        /// <value>
        /// The dynamic methods.
        /// </value>
        public IDictionary<string, IEnumerable<IDynamicMethod>> DynamicMethods
        {
            get { return this.dynamicMethods.ToDictionary(kv => kv.Key, kv => (IEnumerable<IDynamicMethod>)kv.Value); }
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
        public object GetValue(object instance, string propertyName)
        {
            if (instance == null)
            {
                return Undefined.Value;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName);
            return dynamicProperty.GetValue(instance);
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
        public object TryGetValue(object instance, string propertyName)
        {
            if (instance == null)
            {
                return Undefined.Value;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName, throwOnNotFound: false);
            return dynamicProperty == null ? Undefined.Value : dynamicProperty.GetValue(instance);
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
        public void SetValue(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                return;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName);
            dynamicProperty.SetValue(instance, value);
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
        public bool TrySetValue(object instance, string propertyName, object value)
        {
            if (instance == null)
            {
                return false;
            }

            var dynamicProperty = this.GetDynamicProperty(propertyName, throwOnNotFound: false);
            if (dynamicProperty != null)
            {
                dynamicProperty.SetValue(instance, value);
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

            var matchingMethod = this.GetMatchingMethod(methodName, args);
            return matchingMethod.Invoke(instance, args);
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

            var matchingMethod = this.GetMatchingMethod(methodName, args, throwOnNotFound: false);
            if (matchingMethod != null)
            {
                return matchingMethod.TryInvoke(instance, args);
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
        private static IDictionary<string, IList<IDynamicMethod>> CreateDynamicMethods(Type type)
        {
            var methodInfos = type.GetRuntimeMethods().Where(mi => !mi.IsStatic).GroupBy(mi => mi.Name, (name, methods) => new KeyValuePair<string, IList<IDynamicMethod>>(name, methods.Select(mi => (IDynamicMethod)new RuntimeDynamicMethod(mi)).ToList()));
            return methodInfos.ToDictionary(g => g.Key, g => g.Value);
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
            var propertyInfos = type.GetRuntimeProperties().Where(p => p.GetMethod != null && !p.GetMethod.IsStatic);
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
                    throw new MemberAccessException($"Property {propertyName} not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return dynamicProperty;
        }

        /// <summary>
        /// Gets the dynamic methods for the provided method name.
        /// </summary>
        /// <param name="methodName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the method is not found.</param>
        /// <returns>
        /// The dynamic method.
        /// </returns>
        private IList<IDynamicMethod> GetDynamicMethods(string methodName, bool throwOnNotFound = true)
        {
            IList<IDynamicMethod> dynamicMethod;
            if (!this.dynamicMethods.TryGetValue(methodName, out dynamicMethod))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException($"Method {methodName} not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return dynamicMethod;
        }

        /// <summary>
        /// Gets the matching method.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the method is not found.</param>
        /// <returns>
        /// A mathing method for the provided name and arguments.
        /// </returns>
        private IDynamicMethod GetMatchingMethod(string methodName, IEnumerable<object> args, bool throwOnNotFound = true)
        {
            var methods = this.GetDynamicMethods(methodName, throwOnNotFound);
            if (methods == null)
            {
                return null;
            }

            if (methods.Count == 1)
            {
                return methods[0];
            }

            var argsCount = args.Count();
            var matchingMethods = methods.Where(mi => mi.MethodInfo.GetParameters().Length == argsCount)
                                        .Take(2)
                                        .ToList();

            if (matchingMethods.Count > 1)
            {
                throw new AmbiguousMatchException($"Multiple methods found with name {methodName} and {argsCount} arguments in {this.Type}.");
            }

            if (matchingMethods.Count == 0)
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException($"Method {methodName} with {argsCount} arguments not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return matchingMethods[0];
        }
    }
}
