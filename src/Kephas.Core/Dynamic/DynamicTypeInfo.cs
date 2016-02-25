// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeInfo.cs" company="Quartz Software SRL">
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
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Provides optimized access to methods and properties at runtime.
    /// </summary>
    public sealed class DynamicTypeInfo : Expando, IDynamicTypeInfo
    {
        /// <summary>
        /// The cache of dynamic type infos.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, DynamicTypeInfo> DynamicTypeInfosCache = new ConcurrentDictionary<Type, DynamicTypeInfo>();

        /// <summary>
        /// The <see cref="DynamicPropertyInfo{T,TMember}"/> generic type information.
        /// </summary>
        private static readonly TypeInfo DynamicPropertyInfoGenericTypeInfo = typeof(DynamicPropertyInfo<,>).GetTypeInfo();

        /// <summary>
        /// The dynamic type of the <see cref="DynamicTypeInfo"/>.
        /// </summary>
        private static readonly DynamicTypeInfo DynamicTypeInfoOfDynamicTypeInfo;

        /// <summary>
        /// The properties.
        /// </summary>
        private IDictionary<string, IDynamicPropertyInfo> properties;

        /// <summary>
        /// The methods.
        /// </summary>
        private IDictionary<string, IEnumerable<IDynamicMethodInfo>> methods;

        /// <summary>
        /// Initializes static members of the <see cref="DynamicTypeInfo"/> class.
        /// </summary>
        static DynamicTypeInfo()
        {
            DynamicTypeInfoOfDynamicTypeInfo = new DynamicTypeInfo(typeof(DynamicTypeInfo));
            DynamicTypeInfosCache.TryAdd(typeof(DynamicTypeInfo), DynamicTypeInfoOfDynamicTypeInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal DynamicTypeInfo(Type type)
            : this(type, type.GetTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeInfo"/> class.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        internal DynamicTypeInfo(TypeInfo typeInfo)
            : this(typeInfo.AsType(), typeInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        private DynamicTypeInfo(Type type, TypeInfo typeInfo)
            : base(isThreadSafe: true)
        {
            this.Type = type;
            this.TypeInfo = typeInfo;
            this.Name = type.Name;
            this.FullName = typeInfo.FullName;
            this.Namespace = type.Namespace;
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public string FullName { get; }

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public string Namespace { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.TypeInfo.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer => null;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Type Type { get; }

        /// <summary>
        /// Gets the underlying <see cref="TypeInfo"/>.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public TypeInfo TypeInfo { get; }

        /// <summary>
        /// Gets the dynamic properties.
        /// </summary>
        /// <value>
        /// The dynamic properties.
        /// </value>
        public IDictionary<string, IDynamicPropertyInfo> Properties => this.GetProperties();

        /// <summary>
        /// Gets the dynamic methods.
        /// </summary>
        /// <value>
        /// The dynamic methods.
        /// </value>
        public IDictionary<string, IEnumerable<IDynamicMethodInfo>> Methods => this.GetMethods();

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo() => this.TypeInfo;

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
            var dynamicProperty = this.GetProperty(propertyName);
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

            var dynamicProperty = this.GetProperty(propertyName, throwOnNotFound: false);
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
            var dynamicProperty = this.GetProperty(propertyName);
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

            var dynamicProperty = this.GetProperty(propertyName, throwOnNotFound: false);
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
        internal static DynamicTypeInfo GetDynamicType(Type type)
        {
            Contract.Requires(type != null);

            return DynamicTypeInfosCache.GetOrAdd(type, _ => new DynamicTypeInfo(type));
        }

        /// <summary>
        /// Gets the dynamic type.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>A runtime dynamic type.</returns>
        internal static DynamicTypeInfo GetDynamicType(TypeInfo typeInfo)
        {
            Contract.Requires(typeInfo != null);

            return DynamicTypeInfosCache.GetOrAdd(typeInfo.AsType(), _ => new DynamicTypeInfo(typeInfo));
        }

        /// <summary>
        /// Gets the dynamic type used by the expando in the dynamic behavior.
        /// </summary>
        /// <returns>
        /// The dynamic type.
        /// </returns>
        protected override IDynamicTypeInfo GetDynamicTypeInfo()
        {
            return DynamicTypeInfoOfDynamicTypeInfo;
        }

        /// <summary>
        /// Creates the dynamic methods.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A dictionary of runtime dynamic methods.</returns>
        private static IDictionary<string, IEnumerable<IDynamicMethodInfo>> CreateMethodInfos(Type type)
        {
            var methodInfos = type.GetRuntimeMethods().Where(mi => !mi.IsStatic).GroupBy(mi => mi.Name, (name, methods) => new KeyValuePair<string, IList<IDynamicMethodInfo>>(name, methods.Select(mi => (IDynamicMethodInfo)new DynamicMethodInfo(mi)).ToList()));
            return new ReadOnlyDictionary<string, IEnumerable<IDynamicMethodInfo>>(methodInfos.ToDictionary(g => g.Key, g => (IEnumerable<IDynamicMethodInfo>)g.Value));
        }

        /// <summary>
        /// Creates the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A dictionary of properties.
        /// </returns>
        private static IDictionary<string, IDynamicPropertyInfo> CreatePropertyInfos(Type type)
        {
            var dynamicProperties = new Dictionary<string, IDynamicPropertyInfo>();
            var propertyInfos = type.GetRuntimeProperties().Where(p => p.GetMethod != null && !p.GetMethod.IsStatic);
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                if (propertyInfo.GetIndexParameters().Length > 0 || dynamicProperties.ContainsKey(propertyName))
                {
                    continue;
                }

                var propertyAccessorType = DynamicPropertyInfoGenericTypeInfo.MakeGenericType(
                    type,
                    propertyInfo.PropertyType);
                var constructor = propertyAccessorType.GetTypeInfo().DeclaredConstructors.First();
                var propertyAccessor = (IDynamicPropertyInfo)constructor.Invoke(new object[] { propertyInfo });
                dynamicProperties.Add(propertyName, propertyAccessor);
            }

            return new ReadOnlyDictionary<string, IDynamicPropertyInfo>(dynamicProperties);
        }

        /// <summary>
        /// Gets the property infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The properties.
        /// </returns>
        private IDictionary<string, IDynamicPropertyInfo> GetProperties()
        {
            return this.properties ?? (this.properties = CreatePropertyInfos(this.Type));
        }

        /// <summary>
        /// Gets the method infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The dynamic method.
        /// </returns>
        private IDictionary<string, IEnumerable<IDynamicMethodInfo>> GetMethods()
        {
            return this.methods ?? (this.methods = CreateMethodInfos(this.Type));
        }

        /// <summary>
        /// Gets the dynamic property for the provided property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the property is not found.</param>
        /// <returns>
        /// The dynamic property.
        /// </returns>
        private IDynamicPropertyInfo GetProperty(string propertyName, bool throwOnNotFound = true)
        {
            IDynamicPropertyInfo propertyInfo;
            if (!this.GetProperties().TryGetValue(propertyName, out propertyInfo))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException($"Property {propertyName} not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return propertyInfo;
        }

        /// <summary>
        /// Gets the dynamic methods for the provided method name.
        /// </summary>
        /// <param name="methodName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the method is not found.</param>
        /// <returns>
        /// The dynamic method.
        /// </returns>
        private IList<IDynamicMethodInfo> GetMethods(string methodName, bool throwOnNotFound = true)
        {
            IEnumerable<IDynamicMethodInfo> methodInfos;
            if (!this.GetMethods().TryGetValue(methodName, out methodInfos))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException($"Method {methodName} not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return (IList<IDynamicMethodInfo>)methodInfos;
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
        private IDynamicMethodInfo GetMatchingMethod(string methodName, IEnumerable<object> args, bool throwOnNotFound = true)
        {
            var methodInfos = this.GetMethods(methodName, throwOnNotFound);
            if (methodInfos == null)
            {
                return null;
            }

            if (methodInfos.Count == 1)
            {
                return methodInfos[0];
            }

            var argsCount = args.Count();
            var matchingMethods = methodInfos.Where(mi => mi.MethodInfo.GetParameters().Length == argsCount)
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
