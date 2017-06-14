// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Provides optimized access to methods and properties at runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// An object activator delegate.
    /// </summary>
    /// <param name="args">A variable-length parameters list containing arguments.</param>
    /// <returns>
    /// An object.
    /// </returns>
    internal delegate object InstanceActivator(params object[] args);

    /// <summary>
    /// Provides optimized access to methods and properties at runtime.
    /// </summary>
    public sealed class RuntimeTypeInfo : Expando, IRuntimeTypeInfo
    {
        /// <summary>
        /// The cache of runtime type infos.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, IRuntimeTypeInfo> RuntimeTypeInfosCache = new ConcurrentDictionary<Type, IRuntimeTypeInfo>();

        /// <summary>
        /// The <see cref="RuntimeFieldInfo{T,TMember}"/> generic type information.
        /// </summary>
        private static readonly TypeInfo RuntimeFieldInfoGenericTypeInfo = typeof(RuntimeFieldInfo<,>).GetTypeInfo();

        /// <summary>
        /// The <see cref="RuntimeFieldInfo"/> type information.
        /// </summary>
        private static readonly TypeInfo RuntimeFieldInfoTypeInfo = typeof(RuntimeFieldInfo).GetTypeInfo();

        /// <summary>
        /// The <see cref="RuntimePropertyInfo{T,TMember}"/> generic type information.
        /// </summary>
        private static readonly TypeInfo RuntimePropertyInfoGenericTypeInfo = typeof(RuntimePropertyInfo<,>).GetTypeInfo();

        /// <summary>
        /// The <see cref="RuntimePropertyInfo"/> type information.
        /// </summary>
        private static readonly TypeInfo RuntimePropertyInfoTypeInfo = typeof(RuntimePropertyInfo).GetTypeInfo();

        /// <summary>
        /// The runtime type of the <see cref="RuntimeTypeInfo"/>.
        /// </summary>
        private static readonly RuntimeTypeInfo RuntimeTypeInfoOfRuntimeTypeInfo;

        /// <summary>
        /// The function for creating the type info.
        /// </summary>
        private static Func<Type, IRuntimeTypeInfo> createRuntimeTypeInfoFunc = t => new RuntimeTypeInfo(t);

        /// <summary>
        /// The fields.
        /// </summary>
        private IDictionary<string, IRuntimeFieldInfo> fields;

        /// <summary>
        /// The properties.
        /// </summary>
        private IDictionary<string, IRuntimePropertyInfo> properties;

        /// <summary>
        /// The methods.
        /// </summary>
        private IDictionary<string, ICollection<IRuntimeMethodInfo>> methods;

        /// <summary>
        /// The members.
        /// </summary>
        private IDictionary<string, IRuntimeElementInfo> members;

        /// <summary>
        /// The base <see cref="ITypeInfo"/>s.
        /// </summary>
        private IReadOnlyList<ITypeInfo> baseTypes;

        /// <summary>
        /// The generic type parameters.
        /// </summary>
        private IReadOnlyList<ITypeInfo> genericTypeParameters;

        /// <summary>
        /// The generic type arguments.
        /// </summary>
        private IReadOnlyList<ITypeInfo> genericTypeArguments;

        /// <summary>
        /// The generic type definition.
        /// </summary>
        private ITypeInfo genericTypeDefinition;

        /// <summary>
        /// The default value.
        /// </summary>
        private object defaultValue;

        /// <summary>
        /// True if default value created.
        /// </summary>
        private bool defaultValueCreated;

        /// <summary>
        /// The instance activator.
        /// </summary>
        private InstanceActivator instanceActivator;

        /// <summary>
        /// The declaring container.
        /// </summary>
        private IRuntimeAssemblyInfo declaringContainer;

        /// <summary>
        /// Initializes static members of the <see cref="RuntimeTypeInfo"/> class.
        /// </summary>
        static RuntimeTypeInfo()
        {
            RuntimeTypeInfoOfRuntimeTypeInfo = new RuntimeTypeInfo(typeof(RuntimeTypeInfo));
            RuntimeTypeInfosCache.TryAdd(typeof(RuntimeTypeInfo), RuntimeTypeInfoOfRuntimeTypeInfo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        internal RuntimeTypeInfo(Type type)
            : this(type, type.GetTypeInfo())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeInfo"/> class.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        internal RuntimeTypeInfo(TypeInfo typeInfo)
            : this(typeInfo.AsType(), typeInfo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeInfo"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        private RuntimeTypeInfo(Type type, TypeInfo typeInfo)
            : base(isThreadSafe: true)
        {
            this.Type = type;
            this.TypeInfo = typeInfo;
            this.Name = type.Name;
            this.FullName = typeInfo.FullName;
            this.Namespace = type.Namespace;
        }

        /// <summary>
        /// Gets or sets the function for creating the runtime type information.
        /// </summary>
        /// <value>
        /// The function for creating the runtime type information.
        /// </value>
        public static Func<Type, IRuntimeTypeInfo> CreateRuntimeTypeInfo
        {
            get => createRuntimeTypeInfoFunc;
            set
            {
                Requires.NotNull(value, nameof(value));
                createRuntimeTypeInfoFunc = value;
            }
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
        /// Gets the full name qualified with the module where it was defined.
        /// </summary>
        /// <value>
        /// The full name qualified with the module.
        /// </value>
        public string QualifiedFullName => this.TypeInfo.GetQualifiedFullName();

        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public string Namespace { get; }

        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <value>
        /// The default value.
        /// </value>
        public object DefaultValue
        {
            get
            {
                if (!this.defaultValueCreated && this.TypeInfo.IsValueType && this.Type != typeof(void))
                {
                    this.defaultValue = Activator.CreateInstance(this.Type);
                    this.defaultValueCreated = true;
                }

                return this.defaultValue;
            }
        }

        /// <summary>
        /// Gets the runtime type kind.
        /// </summary>
        /// <value>
        /// The runtime type kind.
        /// </value>
        public RuntimeTypeKind Kind => this.TypeInfo.IsClass
                                           ? RuntimeTypeKind.Class
                                           : this.TypeInfo.IsInterface
                                               ? RuntimeTypeKind.Interface
                                               : this.TypeInfo.IsEnum
                                                   ? RuntimeTypeKind.Enum
                                                   : RuntimeTypeKind.Unknown;

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        public IEnumerable<ITypeInfo> BaseTypes => this.baseTypes ?? (this.baseTypes = this.CreateBaseTypes(this.TypeInfo));

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeParameters => this.genericTypeParameters ?? (this.genericTypeParameters = this.CreateGenericTypeParameters(this.TypeInfo));

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type arguments of a closed generic type.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeArguments => this.genericTypeArguments ?? (this.genericTypeArguments = this.CreateGenericTypeArguments(this.TypeInfo));

        /// <summary>
        /// Gets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        public ITypeInfo GenericTypeDefinition => this.genericTypeDefinition ?? (this.genericTypeDefinition = this.GetGenericTypeDefinition(this.TypeInfo));

        /// <summary>
        /// Gets the enumeration of properties.
        /// </summary>
        IEnumerable<IPropertyInfo> ITypeInfo.Properties => this.Properties.Values;

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.Members.Values;

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
        public IElementInfo DeclaringContainer => this.declaringContainer ?? (this.declaringContainer = RuntimeAssemblyInfo.GetRuntimeAssembly(this.TypeInfo.Assembly));

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
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public IDictionary<string, IRuntimeElementInfo> Members => this.GetMembers();

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>
        /// The fields.
        /// </value>
        public IDictionary<string, IRuntimeFieldInfo> Fields => this.GetFields();

        /// <summary>
        /// Gets the runtime properties.
        /// </summary>
        /// <value>
        /// The runtime properties.
        /// </value>
        public IDictionary<string, IRuntimePropertyInfo> Properties => this.GetProperties();

        /// <summary>
        /// Gets the runtime methods.
        /// </summary>
        /// <value>
        /// The runtime methods.
        /// </value>
        public IDictionary<string, ICollection<IRuntimeMethodInfo>> Methods => this.GetMethods();

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        public MemberInfo GetUnderlyingMemberInfo() => this.TypeInfo;

        /// <summary>
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        public IElementInfo GetMember(string name, bool throwIfNotFound = true)
        {
            if (this.Fields.TryGetValue(name, out IRuntimeFieldInfo fieldInfo))
            {
                return fieldInfo;
            }

            if (this.Properties.TryGetValue(name, out IRuntimePropertyInfo propertyInfo))
            {
                return propertyInfo;
            }

            if (this.Methods.TryGetValue(name, out ICollection<IRuntimeMethodInfo> methodInfos))
            {
                if (methodInfos.Count > 1)
                {
                    throw new AmbiguousMatchException(string.Format(Strings.RuntimeTypeInfo_AmbiguousMember_Exception, name, this, nameof(this.Methods)));
                }

                return methodInfos.First();
            }

            if (throwIfNotFound)
            {
                throw new KeyNotFoundException(string.Format(Strings.RuntimeTypeInfo_MemberNotFound_Exception, name, this.Type));
            }

            return null;
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
            var dynamicProperty = this.GetProperty(propertyName);
            return dynamicProperty.GetValue(instance);
        }

        /// <summary>
        /// Gets the value of the property with the specified name.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The property value.</param>
        /// <returns>
        /// A boolean value indicating whether the property is found.
        /// </returns>
        public bool TryGetValue(object instance, string propertyName, out object value)
        {
            Requires.NotNull(instance, nameof(instance));

            var dynamicProperty = this.GetProperty(propertyName, throwOnNotFound: false);
            value = dynamicProperty?.GetValue(instance);
            return dynamicProperty != null;
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
        /// <param name="result">The invocation result.</param>
        /// <returns>A boolean value indicating whether the invocation was successful or not.</returns>
        public bool TryInvoke(object instance, string methodName, IEnumerable<object> args, out object result)
        {
            Requires.NotNull(instance, nameof(instance));

            var matchingMethod = this.GetMatchingMethod(methodName, args, throwOnNotFound: false);
            if (matchingMethod == null)
            {
                result = null;
                return false;
            }

            result = matchingMethod.Invoke(instance, args);
            return true;
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public object CreateInstance(IEnumerable<object> args = null)
        {
            if (this.instanceActivator == null)
            {
                this.instanceActivator = this.CreateInstanceActivator();
            }

            return args == null ? this.instanceActivator() : this.instanceActivator(args.ToArray());
        }

        /// <summary>
        /// Constructs a generic type baed on the provided type arguments.
        /// </summary>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="constructionContext">The construction context (optional).</param>
        /// <returns>
        /// A constructed <see cref="ITypeInfo"/>.
        /// </returns>
        public ITypeInfo MakeGenericType(IEnumerable<ITypeInfo> typeArguments, IContext constructionContext = null)
        {
            var constructedType = this.TypeInfo.MakeGenericType(typeArguments.Cast<IRuntimeTypeInfo>().Select(t => t.Type).ToArray());
            return constructedType.AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Type.ToString();
        }

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        public IEnumerable<TAttribute> GetAttributes<TAttribute>()
            where TAttribute : Attribute
        {
            return this.TypeInfo.GetCustomAttributes<TAttribute>(inherit: true);
        }

        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime type.</returns>
        internal static IRuntimeTypeInfo GetRuntimeType(Type type)
        {
            Requires.NotNull(type, nameof(type));

            return RuntimeTypeInfosCache.GetOrAdd(type, _ => CreateRuntimeTypeInfo(type) ?? new RuntimeTypeInfo(type));
        }

        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>A runtime type.</returns>
        internal static IRuntimeTypeInfo GetRuntimeType(TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return RuntimeTypeInfosCache.GetOrAdd(typeInfo.AsType(), _ => CreateRuntimeTypeInfo(typeInfo.AsType()) ?? new RuntimeTypeInfo(typeInfo));
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> of this expando object.
        /// </summary>
        /// <returns>
        /// The <see cref="IRuntimeTypeInfo"/> of this expando object.
        /// </returns>
        protected override ITypeInfo GetThisTypeInfo()
        {
            return RuntimeTypeInfoOfRuntimeTypeInfo;
        }

        /// <summary>
        /// Creates the runtime method infos.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A dictionary of runtime method infos.</returns>
        private static IDictionary<string, ICollection<IRuntimeMethodInfo>> CreateMethodInfos(Type type)
        {
            var methodInfos = type.GetRuntimeMethods()
                                .Where(mi => !mi.IsStatic && mi.IsPublic)
                                .GroupBy(mi => mi.Name, (name, methods) => new KeyValuePair<string, ICollection<IRuntimeMethodInfo>>(name, new ReadOnlyCollection<IRuntimeMethodInfo>(methods.Select(mi => (IRuntimeMethodInfo)new RuntimeMethodInfo(mi)).ToList())));
            return new ReadOnlyDictionary<string, ICollection<IRuntimeMethodInfo>>(methodInfos.ToDictionary(g => g.Key, g => g.Value));
        }

        /// <summary>
        /// Creates the member infos.
        /// </summary>
        /// <param name="fieldInfos">The field infos.</param>
        /// <param name="propertyInfos">The property infos.</param>
        /// <param name="methodInfos">The method infos.</param>
        /// <returns>
        /// The new member infos.
        /// </returns>
        private static IDictionary<string, IRuntimeElementInfo> CreateMemberInfos(
            IDictionary<string, IRuntimeFieldInfo> fieldInfos,
            IDictionary<string, IRuntimePropertyInfo> propertyInfos,
            IDictionary<string, ICollection<IRuntimeMethodInfo>> methodInfos)
        {
            var memberInfos = new Dictionary<string, IRuntimeElementInfo>();
            foreach (var kv in fieldInfos)
            {
                memberInfos.Add(kv.Key, kv.Value);
            }

            foreach (var kv in propertyInfos)
            {
                memberInfos.Add(kv.Key, kv.Value);
            }

            foreach (var kv in methodInfos)
            {
                var groupMethodInfos = (IList<IRuntimeMethodInfo>)kv.Value;
                if (groupMethodInfos.Count > 1)
                {
                    for (var i = 0; i < groupMethodInfos.Count; i++)
                    {
                        var methodInfo = groupMethodInfos[i];
                        memberInfos.Add($"{kv.Key}#{i}", methodInfo);
                    }
                }
                else
                {
                    memberInfos.Add(groupMethodInfos[0].Name, groupMethodInfos[0]);
                }
            }

            return new ReadOnlyDictionary<string, IRuntimeElementInfo>(memberInfos);
        }

        /// <summary>
        /// Creates the fields.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A dictionary of fields.
        /// </returns>
        private static IDictionary<string, IRuntimeFieldInfo> CreateFieldInfos(Type type)
        {
            var runtimeFieldInfos = new Dictionary<string, IRuntimeFieldInfo>();
            var fieldInfos = type.GetRuntimeFields().Where(f => f.IsPublic);
            var fieldAccessorTypeFn = type.GetTypeInfo().ContainsGenericParameters
                                             ? (Func<Type, Type>)(fieldType => RuntimeFieldInfoTypeInfo.AsType())
                                             : (fieldType => RuntimeFieldInfoGenericTypeInfo.MakeGenericType(type, fieldType));

            foreach (var fieldInfo in fieldInfos)
            {
                var fieldName = fieldInfo.Name;
                if (runtimeFieldInfos.ContainsKey(fieldName))
                {
                    continue;
                }

                var fieldAccessorType = fieldAccessorTypeFn(fieldInfo.FieldType);
                var constructor = fieldAccessorType.GetTypeInfo().DeclaredConstructors.First();
                var runtimeFieldInfo = (IRuntimeFieldInfo)constructor.Invoke(new object[] { fieldInfo });
                runtimeFieldInfos.Add(fieldName, runtimeFieldInfo);
            }

            return new ReadOnlyDictionary<string, IRuntimeFieldInfo>(runtimeFieldInfos);
        }

        /// <summary>
        /// Creates the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A dictionary of properties.
        /// </returns>
        private static IDictionary<string, IRuntimePropertyInfo> CreatePropertyInfos(Type type)
        {
            var runtimePropertyInfos = new Dictionary<string, IRuntimePropertyInfo>();
            var propertyInfos = type.GetRuntimeProperties().Where(p => p.GetMethod != null && !p.GetMethod.IsStatic && p.GetMethod.IsPublic);
            var propertyAccessorTypeFn = type.GetTypeInfo().ContainsGenericParameters
                                                ? (Func<Type, Type>)(propType => RuntimePropertyInfoTypeInfo.AsType())
                                                : (propType => RuntimePropertyInfoGenericTypeInfo.MakeGenericType(type, propType));
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyName = propertyInfo.Name;
                if (propertyInfo.GetIndexParameters().Length > 0 || runtimePropertyInfos.ContainsKey(propertyName))
                {
                    continue;
                }

                var propertyAccessorType = propertyAccessorTypeFn(propertyInfo.PropertyType);
                var constructor = propertyAccessorType.GetTypeInfo().DeclaredConstructors.First();
                var runtimePropertyInfo = (IRuntimePropertyInfo)constructor.Invoke(new object[] { propertyInfo });
                runtimePropertyInfos.Add(propertyName, runtimePropertyInfo);
            }

            return new ReadOnlyDictionary<string, IRuntimePropertyInfo>(runtimePropertyInfos);
        }

        /// <summary>
        /// Creates the bases.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>
        /// The new bases.
        /// </returns>
        private IReadOnlyList<ITypeInfo> CreateBaseTypes(TypeInfo typeInfo)
        {
            var baseTypeInfos = new List<ITypeInfo>();
            if (typeInfo.BaseType != null)
            {
                baseTypeInfos.Add(typeInfo.BaseType.AsRuntimeTypeInfo());
            }

            typeInfo.ImplementedInterfaces.ForEach(i => baseTypeInfos.Add(i.AsRuntimeTypeInfo()));

            return new ReadOnlyCollection<ITypeInfo>(baseTypeInfos);
        }

        /// <summary>
        /// Gets the generic type definition.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>
        /// The generic type definition.
        /// </returns>
        private ITypeInfo GetGenericTypeDefinition(TypeInfo typeInfo)
        {
            if (typeInfo.IsGenericType && !typeInfo.IsGenericTypeDefinition)
            {
                return GetRuntimeType(typeInfo.GetGenericTypeDefinition());
            }

            return null;
        }

        /// <summary>
        /// Gets the members, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The members.
        /// </returns>
        private IDictionary<string, IRuntimeElementInfo> GetMembers()
        {
            return this.members ?? (this.members = CreateMemberInfos(this.Fields, this.Properties, this.Methods));
        }

        /// <summary>
        /// Gets the field infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The fields.
        /// </returns>
        private IDictionary<string, IRuntimeFieldInfo> GetFields()
        {
            return this.fields ?? (this.fields = CreateFieldInfos(this.Type));
        }

        /// <summary>
        /// Gets the property infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The properties.
        /// </returns>
        private IDictionary<string, IRuntimePropertyInfo> GetProperties()
        {
            return this.properties ?? (this.properties = CreatePropertyInfos(this.Type));
        }

        /// <summary>
        /// Gets the method infos, initializing them if necessary.
        /// </summary>
        /// <returns>
        /// The dynamic method.
        /// </returns>
        private IDictionary<string, ICollection<IRuntimeMethodInfo>> GetMethods()
        {
            return this.methods ?? (this.methods = CreateMethodInfos(this.Type));
        }

        /// <summary>
        /// Creates the generic parameters.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>
        /// The new generic arguments.
        /// </returns>
        private IReadOnlyList<ITypeInfo> CreateGenericTypeParameters(TypeInfo typeInfo)
        {
            var args = typeInfo.GenericTypeParameters;
            if (args.Length == 0)
            {
                return ReflectionHelper.EmptyTypeInfos;
            }

            return new ReadOnlyCollection<ITypeInfo>(args.Select(a => (ITypeInfo)GetRuntimeType(a)).ToList());
        }

        /// <summary>
        /// Creates the generic arguments.
        /// </summary>
        /// <param name="typeInfo">The <see cref="TypeInfo"/>.</param>
        /// <returns>
        /// The new generic arguments.
        /// </returns>
        private IReadOnlyList<ITypeInfo> CreateGenericTypeArguments(TypeInfo typeInfo)
        {
            var args = typeInfo.GenericTypeArguments;
            if (args.Length == 0)
            {
                return ReflectionHelper.EmptyTypeInfos;
            }

            return new ReadOnlyCollection<ITypeInfo>(args.Select(a => (ITypeInfo)GetRuntimeType(a)).ToList());
        }

        /// <summary>
        /// Gets the runtime property for the provided property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the property is not found.</param>
        /// <returns>
        /// The runtime property.
        /// </returns>
        private IRuntimePropertyInfo GetProperty(string propertyName, bool throwOnNotFound = true)
        {
            if (!this.GetProperties().TryGetValue(propertyName, out IRuntimePropertyInfo propertyInfo))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException(string.Format(Strings.RuntimeTypeInfo_PropertyNotFound_Exception, propertyName, this.Type));
                }

                return null;
            }

            return propertyInfo;
        }

        /// <summary>
        /// Gets the runtime methods for the provided method name.
        /// </summary>
        /// <param name="methodName">Name of the property.</param>
        /// <param name="throwOnNotFound">If set to <c>true</c> an exception is thrown if the method is not found.</param>
        /// <returns>
        /// The runtime method.
        /// </returns>
        private IList<IRuntimeMethodInfo> GetMethods(string methodName, bool throwOnNotFound = true)
        {
            if (!this.GetMethods().TryGetValue(methodName, out ICollection<IRuntimeMethodInfo> methodInfos))
            {
                if (throwOnNotFound)
                {
                    throw new MemberAccessException($"Method {methodName} not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return (IList<IRuntimeMethodInfo>)methodInfos;
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
        private IRuntimeMethodInfo GetMatchingMethod(string methodName, IEnumerable<object> args, bool throwOnNotFound = true)
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
                // TODO localization
                throw new AmbiguousMatchException($"Multiple methods found with name {methodName} and {argsCount} arguments in {this.Type}.");
            }

            if (matchingMethods.Count == 0)
            {
                if (throwOnNotFound)
                {
                    // TODO localization
                    throw new MemberAccessException($"Method {methodName} with {argsCount} arguments not found or is not accessible in {this.Type}.");
                }

                return null;
            }

            return matchingMethods[0];
        }

        /// <summary>
        /// Creates an optimized instance activator.
        /// </summary>
        /// <returns>
        /// The new instance activator.
        /// </returns>
        private InstanceActivator CreateInstanceActivator()
        {
            var constructors = this.TypeInfo.DeclaredConstructors.ToList();

            if (constructors.Count == 0)
            {
                return args => { throw new InvalidOperationException(string.Format(Strings.RuntimeTypeInfo_NoPublicConstructorDefined_Exception, this.Type)); };
            }

            if (constructors.Count > 1)
            {
                return args => args == null ? Activator.CreateInstance(this.Type) : Activator.CreateInstance(this.Type, args);
            }

            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            var argsParam = Expression.Parameter(typeof(object[]), "args");

            var argex = new Expression[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var index = Expression.Constant(i);
                var paramType = parameters[i].ParameterType;
                var accessor = Expression.ArrayIndex(argsParam, index);
                var cast = Expression.Convert(accessor, paramType);
                argex[i] = cast;
            }

            var newex = Expression.New(constructor, argex);
            var lambda = Expression.Lambda(typeof(InstanceActivator), newex, argsParam);
            var activator = (InstanceActivator)lambda.Compile();
            return activator;
        }
    }
}
