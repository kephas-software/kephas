// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Provides methods for accessing runtime type information.
    /// </summary>
    public class RuntimeTypeRegistry : IRuntimeTypeRegistry
    {
        private readonly ConcurrentDictionary<Type, IRuntimeTypeInfo> runtimeTypeInfosCache;

        private readonly IList<IRuntimeTypeInfoFactory> typeFactories
            = new List<IRuntimeTypeInfoFactory>();

        private readonly ConcurrentDictionary<Assembly, IRuntimeAssemblyInfo> runtimeAssemblyInfosCache
            = new ConcurrentDictionary<Assembly, IRuntimeAssemblyInfo>();

        private Func<Assembly, IRuntimeAssemblyInfo> createRuntimeAssemblyInfoFunc;

        /// <summary>
        /// The function for creating the type info.
        /// </summary>
        private Func<Type, IRuntimeTypeInfo> createRuntimeTypeInfoFunc;

        internal readonly RuntimeTypeInfo TypeOfRuntimeTypeInfo;
        internal readonly RuntimeTypeInfo TypeOfRuntimePropertyInfo;
        internal readonly RuntimeTypeInfo TypeOfRuntimeFieldInfo;
        internal readonly RuntimeTypeInfo TypeOfRuntimeMethodInfo;
        internal readonly RuntimeTypeInfo TypeOfRuntimeParameterInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeRegistry"/> class.
        /// </summary>
        public RuntimeTypeRegistry()
        {
            this.runtimeTypeInfosCache = new ConcurrentDictionary<Type, IRuntimeTypeInfo>(
                new Dictionary<Type, IRuntimeTypeInfo>
                {
                    { typeof(RuntimeTypeInfo), this.TypeOfRuntimeTypeInfo = new RuntimeTypeInfo(this, typeof(RuntimeTypeInfo)) },
                    { typeof(RuntimePropertyInfo), this.TypeOfRuntimePropertyInfo = new RuntimeTypeInfo(this, typeof(RuntimePropertyInfo)) },
                    { typeof(RuntimeFieldInfo), this.TypeOfRuntimeFieldInfo = new RuntimeTypeInfo(this, typeof(RuntimeFieldInfo)) },
                    { typeof(RuntimeMethodInfo), this.TypeOfRuntimeMethodInfo = new RuntimeTypeInfo(this, typeof(RuntimeMethodInfo)) },
                    { typeof(RuntimeMethodInfo), this.TypeOfRuntimeParameterInfo = new RuntimeTypeInfo(this, typeof(RuntimeParameterInfo)) },
                });

            this.createRuntimeTypeInfoFunc = this.CreateRuntimeTypeInfoCore;
            this.createRuntimeAssemblyInfoFunc = a => new RuntimeAssemblyInfo(this, a);
        }

        /// <summary>
        /// Gets the static instance of the type serviceRegistry.
        /// </summary>
        public static IRuntimeTypeRegistry Instance { get; } = new RuntimeTypeRegistry();

        /// <summary>
        /// Gets or sets the function for creating the runtime type information.
        /// </summary>
        /// <value>
        /// The function for creating the runtime type information.
        /// </value>
        public Func<Type, IRuntimeTypeInfo> CreateRuntimeTypeInfo
        {
            get => this.createRuntimeTypeInfoFunc;
            set
            {
                Requires.NotNull(value, nameof(value));
                this.createRuntimeTypeInfoFunc = value;
            }
        }

        /// <summary>
        /// Gets or sets the function for creating the runtime assembly information.
        /// </summary>
        /// <value>
        /// The function for creating the runtime assembly information.
        /// </value>
        public Func<Assembly, IRuntimeAssemblyInfo> CreateRuntimeAssemblyInfo
        {
            get => this.createRuntimeAssemblyInfoFunc;
            set
            {
                Requires.NotNull(value, nameof(value));
                this.createRuntimeAssemblyInfoFunc = value;
            }
        }

        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime type.</returns>
        public IRuntimeTypeInfo GetRuntimeType(Type type)
        {
            Requires.NotNull(type, nameof(type));

            return this.runtimeTypeInfosCache.GetOrAdd(type, _ => this.CreateRuntimeTypeInfo(type) ?? new RuntimeTypeInfo(this, type));
        }

        /// <summary>
        /// Gets the runtime assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A runtime assembly.</returns>
        public IRuntimeAssemblyInfo GetRuntimeAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.runtimeAssemblyInfosCache.GetOrAdd(assembly, _ => this.CreateRuntimeAssemblyInfo(assembly) ?? new RuntimeAssemblyInfo(this, assembly));
        }

        /// <summary>
        /// Registers a factory used to create <see cref="IRuntimeTypeInfo"/> instances.
        /// </summary>
        /// <remarks>
        /// Factories are called in the inverse order of their addition, meaning that the last added factory
        /// is invoked first. This is by design, so that the non-framework code has a change to override the
        /// default behavior.
        /// </remarks>
        /// <param name="factory">The factory.</param>
        public void RegisterFactory(IRuntimeTypeInfoFactory factory)
        {
            Requires.NotNull(factory, nameof(factory));

            this.typeFactories.Insert(0, factory);
        }

        private IRuntimeTypeInfo CreateRuntimeTypeInfoCore(Type rawType)
        {
            if (this.typeFactories.Count > 0)
            {
                foreach (var factory in this.typeFactories)
                {
                    var typeInfo = factory.TryCreateRuntimeTypeInfo(rawType);
                    if (typeInfo != null)
                    {
                        return typeInfo;
                    }
                }
            }

            return new RuntimeTypeInfo(this, rawType);
        }
    }
}