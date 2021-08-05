// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic type information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Resources;
    using Kephas.Serialization;
    using Kephas.Services;

    /// <summary>
    /// Dynamic type information.
    /// </summary>
    public class DynamicTypeInfo : DynamicElementInfo, ITypeInfo, IIdentifiable
    {
        private readonly ICollection<IElementInfo> members;
        private readonly List<ITypeInfo> genericTypeArguments = new ();
        private readonly List<ITypeInfo> genericTypeParameters = new ();
        private List<ITypeInfo>? baseTypes;
        private string? qualifiedFullName;
        private string? bases;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeInfo"/> class.
        /// </summary>
        public DynamicTypeInfo()
        {
            this.members = new DynamicElementInfoCollection<IElementInfo>(this);
        }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public virtual object Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public override string FullName => string.IsNullOrEmpty(this.Namespace) ? base.FullName : $"{this.Namespace}.{this.Name}";

        /// <summary>
        /// Gets or sets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public virtual string? Namespace { get; set; }

        /// <summary>
        /// Gets or sets the full name qualified with the module where it was defined.
        /// </summary>
        /// <value>
        /// The full name qualified with the module.
        /// </value>
        public virtual string QualifiedFullName
        {
            get => this.qualifiedFullName ?? this.FullName;
            protected internal set => this.qualifiedFullName = value;
        }

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        IEnumerable<ITypeInfo> ITypeInfo.BaseTypes
            => this.baseTypes ??= this.bases == null ? new List<ITypeInfo>() : this.ComputeBaseTypes(this.bases);

        /// <summary>
        /// Gets or sets the base type name. Multiple bases are separated by commas.
        /// </summary>
        public virtual string? Base
        {
            get => this.bases ??= this.baseTypes?.Select(b => b.FullName).JoinWith(", ");
            set
            {
                this.bases = value;
                this.baseTypes = null;
            }
        }

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        IReadOnlyList<ITypeInfo> ITypeInfo.GenericTypeParameters => this.genericTypeParameters;

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type arguments of a closed generic type.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        IReadOnlyList<ITypeInfo> ITypeInfo.GenericTypeArguments => this.genericTypeArguments;

        /// <summary>
        /// Gets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        ITypeInfo? ITypeInfo.GenericTypeDefinition { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        [ExcludeFromSerialization]
        public IEnumerable<IPropertyInfo> Properties => this.Members.OfType<IPropertyInfo>();

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        public ICollection<IElementInfo> Members => this.members;

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> ITypeInfo.Members => this.members;

        /// <summary>
        /// Gets the container type registry.
        /// </summary>
        [ExcludeFromSerialization]
        public ITypeRegistry TypeRegistry =>
            this.GetTypeRegistry()
                ?? throw new InvalidOperationException($"The {nameof(this.DeclaringContainer)} is not set. Try add the '{this.GetType()}' to the '{nameof(DynamicTypeRegistry.Types)}' collection.");

        /// <summary>
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        public virtual IElementInfo? GetMember(string name, bool throwIfNotFound = true)
        {
            var memberInfo = this.Members.FirstOrDefault(m => m.Name == name);
            if (memberInfo != null)
            {
                return memberInfo;
            }

            if (throwIfNotFound)
            {
                throw new KeyNotFoundException(string.Format(Strings.RuntimeTypeInfo_MemberNotFound_Exception, name, this));
            }

            return null;
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public virtual object CreateInstance(IEnumerable<object?>? args = null)
        {
            return new Expando();
        }

        /// <summary>
        /// Constructs a generic type based on the provided type arguments.
        /// </summary>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="constructionContext">The construction context (optional).</param>
        /// <returns>
        /// A constructed <see cref="ITypeInfo"/>.
        /// </returns>
        public ITypeInfo MakeGenericType(IEnumerable<ITypeInfo> typeArguments, IContext? constructionContext = null)
        {
            return this;
        }

        /// <summary>
        /// Computes the base types based on the provided bases.
        /// </summary>
        /// <param name="bases">The base type names separated by comma.</param>
        /// <returns>A list of <see cref="ITypeInfo"/>.</returns>
        protected virtual List<ITypeInfo> ComputeBaseTypes(string bases)
        {
            var typeRegistry = this.TypeRegistry;
            return bases.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(b => typeRegistry.GetTypeInfo(b)!)
                .ToList();
        }
    }
}