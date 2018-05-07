// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAssemblyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime assembly information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Reflection;

    /// <summary>
    /// Information about the runtime assembly. This class cannot be inherited.
    /// </summary>
    public sealed class RuntimeAssemblyInfo : Expando, IRuntimeAssemblyInfo
    {
        /// <summary>
        /// Default type loader.
        /// </summary>
        private static readonly ITypeLoader TypeLoader = new DefaultTypeLoader();

        /// <summary>
        /// The cache of runtime assembly infos.
        /// </summary>
        private static readonly ConcurrentDictionary<Assembly, IRuntimeAssemblyInfo> RuntimeAssemblyInfosCache = new ConcurrentDictionary<Assembly, IRuntimeAssemblyInfo>();

        /// <summary>
        /// The function for creating the assembly info.
        /// </summary>
        private static Func<Assembly, IRuntimeAssemblyInfo> createRuntimeAssemblyInfoFunc = a => new RuntimeAssemblyInfo(a);

        /// <summary>
        /// The assembly.
        /// </summary>
        private readonly Assembly assembly;

        /// <summary>
        /// The types.
        /// </summary>
        private IList<ITypeInfo> types;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeAssemblyInfo"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        internal RuntimeAssemblyInfo(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            this.assembly = assembly;
        }

        /// <summary>
        /// Gets or sets the function for creating the runtime assembly information.
        /// </summary>
        /// <value>
        /// The function for creating the runtime assembly information.
        /// </value>
        public static Func<Assembly, IRuntimeAssemblyInfo> CreateRuntimeAssemblyInfo
        {
            get => createRuntimeAssemblyInfoFunc;
            set
            {
                Requires.NotNull(value, nameof(value));
                createRuntimeAssemblyInfoFunc = value;
            }
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        public string Name => this.assembly.GetName().Name;

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public string FullName => this.assembly.FullName;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        public IEnumerable<object> Annotations => this.assembly.GetCustomAttributes();

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        public IElementInfo DeclaringContainer => null;

        /// <summary>
        /// Gets the types declared in this assembly.
        /// </summary>
        /// <value>
        /// The declared types.
        /// </value>
        public IEnumerable<ITypeInfo> Types => this.GetTypes();

        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        ICustomAttributeProvider IRuntimeElementInfo.GetUnderlyingElementInfo()
        {
            return null;
        }

        /// <summary>
        /// Gets the underlying assembly information.
        /// </summary>
        /// <returns>
        /// The underlying assembly information.
        /// </returns>
        public Assembly GetUnderlyingAssemblyInfo()
        {
            return this.assembly;
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
            return this.assembly.GetCustomAttributes<TAttribute>();
        }

        /// <summary>
        /// Gets the runtime assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A runtime assembly.</returns>
        internal static IRuntimeAssemblyInfo GetRuntimeAssembly(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return RuntimeAssemblyInfosCache.GetOrAdd(assembly, _ => CreateRuntimeAssemblyInfo(assembly) ?? new RuntimeAssemblyInfo(assembly));
        }

        /// <summary>
        /// Creates the list of type information from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The types in the assembly.
        /// </returns>
        private static IList<ITypeInfo> CreateTypeInfos(Assembly assembly)
        {
            var types = TypeLoader.GetLoadableExportedTypes(assembly);
            return types.Select(t => (ITypeInfo)RuntimeTypeInfo.GetRuntimeType(t)).ToList();
        }

        /// <summary>
        /// Gets the types in this assembly.
        /// </summary>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        private IEnumerable<ITypeInfo> GetTypes()
        {
            return this.types ?? (this.types = CreateTypeInfos(this.assembly));
        }
    }
}