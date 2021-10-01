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

    using Kephas.Reflection;

    /// <summary>
    /// Information about the runtime assembly. This class cannot be inherited.
    /// </summary>
    public sealed class RuntimeAssemblyInfo : RuntimeElementInfoBase, IRuntimeAssemblyInfo, IEquatable<RuntimeAssemblyInfo>
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
            assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

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
        /// Determines whether the runtime assemblies are based on the same assembly.
        /// </summary>
        /// <param name="left">The object on the left side.</param>
        /// <param name="right">The object on the right side.</param>
        /// <returns>True, if both runtime assemblies are based on the same assembly, otherwise false.</returns>
        public static bool operator ==(RuntimeAssemblyInfo? left, RuntimeAssemblyInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether the runtime assemblies are not based on the same assembly.
        /// </summary>
        /// <param name="left">The object on the left side.</param>
        /// <param name="right">The object on the right side.</param>
        /// <returns>True, if the runtime assemblies are not based on the same assembly, otherwise false.</returns>
        public static bool operator !=(RuntimeAssemblyInfo? left, RuntimeAssemblyInfo? right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Determines whether this runtime assembly is equal to the provided one.
        /// </summary>
        /// <param name="other">The other runtime assembly.</param>
        /// <returns>True, if both runtime assemblies are based on the same assembly, otherwise false.</returns>
        public bool Equals(IRuntimeAssemblyInfo? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.assembly.Equals(other.GetUnderlyingAssemblyInfo());
        }

        /// <summary>
        /// Determines whether this runtime assembly is equal to the provided one.
        /// </summary>
        /// <param name="other">The other runtime assembly.</param>
        /// <returns>True, if both runtime assemblies are based on the same assembly, otherwise false.</returns>
        public bool Equals(RuntimeAssemblyInfo? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.assembly.Equals(other.assembly);
        }


        /// <summary>
        /// Determines whether this runtime assembly is equal to the provided one.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True, if both runtime assemblies are based on the same assembly, otherwise false.</returns>
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is IRuntimeAssemblyInfo other && this.Equals(other));
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.assembly.GetHashCode();
        }

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
            return assemblyTypes.Select(t => (ITypeInfo)this.TypeRegistry.GetTypeInfo(t)).ToList();
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