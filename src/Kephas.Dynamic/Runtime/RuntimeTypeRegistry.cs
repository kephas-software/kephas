// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Dynamic;
    using Kephas.Injection.AttributedModel;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime.Factories;

    /// <summary>
    /// Provides methods for accessing runtime type information.
    /// </summary>
    [ExcludeFromInjection]
    public class RuntimeTypeRegistry : Expando, IRuntimeTypeRegistry, IRuntimeElementInfoFactory
    {
        private readonly ITypeLoader? typeLoader;
        private readonly ConcurrentDictionary<Type, IRuntimeTypeInfo> runtimeTypeInfosCache;
        private readonly ConcurrentDictionary<Assembly, IRuntimeAssemblyInfo> runtimeAssemblyInfosCache = new ();

        private readonly ConcurrentDictionary<Type, List<IRuntimeElementInfoFactory>> factoriesByElementType = new ();

        private Func<Assembly, IRuntimeAssemblyInfo> createRuntimeAssemblyInfoFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeRegistry"/> class.
        /// </summary>
        /// <param name="typeLoader">Optional. The type loader.</param>
        public RuntimeTypeRegistry(ITypeLoader? typeLoader = null)
            : base(isThreadSafe: true)
        {
            this.typeLoader = typeLoader;
            this.runtimeTypeInfosCache = new ConcurrentDictionary<Type, IRuntimeTypeInfo>();

            this.factoriesByElementType.TryAdd(
                typeof(Type),
                new List<IRuntimeElementInfoFactory>() { new DefaultRuntimeTypeInfoFactory() });

            this.createRuntimeAssemblyInfoFunc = a => new RuntimeAssemblyInfo(this, a, this.typeLoader);
        }

        /// <summary>
        /// Gets the static instance of the type serviceRegistry.
        /// </summary>
        public static IRuntimeTypeRegistry Instance { get; } = new RuntimeTypeRegistry(DefaultTypeLoader.Instance);

        /// <summary>
        /// Gets or sets the function for creating the runtime assembly information.
        /// </summary>
        /// <value>
        /// The function for creating the runtime assembly information.
        /// </value>
        public Func<Assembly, IRuntimeAssemblyInfo> CreateRuntimeAssemblyInfo
        {
            get => this.createRuntimeAssemblyInfoFunc;
            set => this.createRuntimeAssemblyInfoFunc = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Tries to create the runtime element information for the provided raw reflection element.
        /// </summary>
        /// <param name="registry">The root type registry.</param>
        /// <param name="reflectInfo">The raw reflection element.</param>
        /// <param name="position">Optional. The position in the declaring container.</param>
        /// <param name="logger">Optional. The logger.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        IRuntimeElementInfo? IRuntimeElementInfoFactory.TryCreateElementInfo(
            IRuntimeTypeRegistry registry,
            MemberInfo reflectInfo,
            int position,
            ILogger? logger)
        {
            if (this != registry)
            {
                throw new ArgumentException($"The '{nameof(registry)}' parameter must match the calling registry.");
            }

            var memberType = reflectInfo.GetType();
            var factoryType = this.factoriesByElementType.GetOrAdd(memberType, mt =>
            {
                var compatibleType = this.factoriesByElementType.Keys.FirstOrDefault(t => t.IsAssignableFrom(memberType));
                return compatibleType == null
                    ? new List<IRuntimeElementInfoFactory>()
                    : this.factoriesByElementType[compatibleType];
            });

            return factoryType
                .Select(factory => factory.TryCreateElementInfo(this, reflectInfo, position, logger))
                .FirstOrDefault(elementInfo => elementInfo != null);
        }

        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <returns>The type information.</returns>
        public ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true)
        {
            typeToken = typeToken ?? throw new ArgumentNullException(nameof(typeToken));

            return typeToken switch
            {
                TypeInfo typeInfo => this.GetTypeInfo(typeInfo),
                Type type => this.GetTypeInfo(type),
                ITypeInfo ti => ti,
                _ => throw new NotSupportedException($"Only type tokens of type '{nameof(Type)}' and '{nameof(TypeInfo)}' are supported, while '{typeToken.GetType()}' was provided.")
            };
        }

        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime type.</returns>
        public IRuntimeTypeInfo GetTypeInfo(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return this.runtimeTypeInfosCache.GetOrAdd(
                type,
                _ => (IRuntimeTypeInfo?)((IRuntimeElementInfoFactory)this).TryCreateElementInfo(this, type)
                     ?? new RuntimeTypeInfo(this, type));
        }

        /// <summary>
        /// Gets the runtime assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A runtime assembly.</returns>
        public IRuntimeAssemblyInfo GetAssemblyInfo(Assembly assembly)
        {
            assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            return this.runtimeAssemblyInfosCache.GetOrAdd(
                assembly,
                _ => this.CreateRuntimeAssemblyInfo(assembly)
                     ?? new RuntimeAssemblyInfo(this, assembly, this.typeLoader));
        }

        /// <summary>
        /// Registers a factory used to create specialized <see cref="IElementInfo"/> instances.
        /// </summary>
        /// <typeparam name="TFactory">The factory type.</typeparam>
        /// <remarks>
        /// Factories are called in the inverse order of their addition, meaning that the last added factory
        /// is invoked first. This is by design, so that the non-framework code has a change to override the
        /// default behavior.
        /// </remarks>
        /// <param name="factory">The factory.</param>
        public void RegisterFactory<TFactory>(TFactory factory)
            where TFactory : class, IRuntimeElementInfoFactory
        {
            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            var factoryType = typeof(TFactory);
            var elementType = factoryType.GetInterfaces()
                .FirstOrDefault(i => i.IsConstructedGenericOf(typeof(IRuntimeElementInfoFactory<,>)))
                ?.GenericTypeArguments[1];
            if (elementType == null)
            {
                throw new NotSupportedException($"The factory {factory.GetType()} must implement the generic {typeof(IRuntimeElementInfoFactory<,>).FullName} interface.");
            }

            var listByElementType = this.factoriesByElementType.GetOrAdd(elementType, t => new List<IRuntimeElementInfoFactory>());
            lock (listByElementType)
            {
                listByElementType.Insert(0, factory);
            }
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ITypeInfo> GetEnumerator() => this.runtimeTypeInfosCache.Values.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.runtimeTypeInfosCache.Values.GetEnumerator();

        private class DefaultRuntimeTypeInfoFactory : RuntimeTypeInfoFactoryBase
        {
            public override IRuntimeTypeInfo? TryCreateElementInfo(IRuntimeTypeRegistry registry, Type reflectInfo, int position = -1, ILogger? logger = null)
                => new RuntimeTypeInfo(registry, reflectInfo, logger);
        }
    }
}