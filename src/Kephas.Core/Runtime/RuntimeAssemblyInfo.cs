// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAssemblyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime assembly information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    /// <summary>
    /// Information about the runtime assembly. This class cannot be inherited.
    /// </summary>
    public sealed class RuntimeAssemblyInfo : RuntimeElementInfoBase, IRuntimeAssemblyInfo
    {
        private readonly Assembly assembly;
        private readonly ITypeLoader typeLoader;

        private IList<ITypeInfo>? types;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeAssemblyInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="assembly">The assembly.</param>
        /// <param name="typeLoader">The type loader.</param>
        internal RuntimeAssemblyInfo(IRuntimeTypeRegistry typeRegistry, Assembly assembly, ITypeLoader? typeLoader)
            : base(typeRegistry)
        {
            Requires.NotNull(assembly, nameof(assembly));

            this.assembly = assembly;
            this.typeLoader = typeLoader ?? DefaultTypeLoader.Instance;
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
        public IElementInfo? DeclaringContainer => null;

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
            return this.assembly;
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

#if NETSTANDARD2_1
#else
        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public IDisplayInfo? GetDisplayInfo() => ElementInfoHelper.GetDisplayInfo(this);
#endif

        /// <summary>
        /// Creates the list of type information from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The types in the assembly.
        /// </returns>
        private IList<ITypeInfo> CreateTypeInfos(Assembly assembly)
        {
            var assemblyTypes = this.typeLoader.GetExportedTypes(assembly);
            return assemblyTypes.Select(t => (ITypeInfo)this.TypeRegistry.GetRuntimeType(t)).ToList();
        }

        /// <summary>
        /// Gets the types in this assembly.
        /// </summary>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        private IEnumerable<ITypeInfo> GetTypes()
        {
            return this.types ??= this.CreateTypeInfos(this.assembly);
        }
    }
}