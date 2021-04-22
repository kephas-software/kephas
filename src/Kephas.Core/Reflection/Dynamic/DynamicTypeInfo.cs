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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// Dynamic type information.
    /// </summary>
    public class DynamicTypeInfo : DynamicElementInfo, ITypeInfo, IIdentifiable
    {
        private readonly ICollection<IElementInfo> members;
        private readonly List<ITypeInfo> genericTypeArguments = new ();
        private readonly List<ITypeInfo> genericTypeParameters = new ();
        private readonly List<ITypeInfo> baseTypes = new ();
        private string? qualifiedFullName;

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
        public object Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        public override string FullName => base.FullName ?? $"{this.Namespace}.{this.Name}";

        /// <summary>
        /// Gets or sets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        public string Namespace { get; set; }

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
        public IEnumerable<ITypeInfo> BaseTypes => this.baseTypes;

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeParameters => this.genericTypeParameters;

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type arguments of a closed generic type.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public IReadOnlyList<ITypeInfo> GenericTypeArguments => this.genericTypeArguments;

        /// <summary>
        /// Gets or sets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        public ITypeInfo GenericTypeDefinition { get; protected internal set; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
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
        public ITypeRegistry TypeRegistry =>
            this.DeclaringContainer as ITypeRegistry
                ?? this.DeclaringContainer?.DeclaringContainer as ITypeRegistry
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
        /// Adds a member to the dynamic type.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="member">The member.</param>
        protected internal virtual void AddMember(IElementInfo member)
        {
            Requires.NotNull(member, nameof(member));
            Requires.NotNullOrEmpty(member.Name, nameof(member.Name));

            if (this.members.Any(m => m.Name == member.Name))
            {
                throw new InvalidOperationException(string.Format(Strings.DynamicTypeInfo_AddMember_Duplicate_Exception, member.Name, this));
            }

            this.members.Add(member);
        }

        /// <summary>
        /// Adds a base type to the dynamic type.
        /// </summary>
        /// <param name="baseType">The base type.</param>
        protected internal virtual void AddBaseType(ITypeInfo baseType)
        {
            Requires.NotNull(baseType, nameof(baseType));

            if (baseType == this)
            {
                throw new ArgumentException(nameof(baseType), string.Format(Strings.DynamicTypeInfo_AddBaseType_TypeCannotBeABaseOfItself_Exception, this));
            }

            this.baseTypes.Add(baseType);
        }

        /// <summary>
        /// Adds a generic type parameter to the dynamic type.
        /// </summary>
        /// <param name="genericTypeParameter">The generic type parameter.</param>
        protected internal virtual void AddGenericTypeParameter(ITypeInfo genericTypeParameter)
        {
            Requires.NotNull(genericTypeParameter, nameof(genericTypeParameter));

            this.genericTypeParameters.Add(genericTypeParameter);
        }

        /// <summary>
        /// Adds a generic type argument to the dynamic type.
        /// </summary>
        /// <param name="genericTypeArgument">The generic type argument.</param>
        protected internal virtual void AddGenericTypeArgument(ITypeInfo genericTypeArgument)
        {
            Requires.NotNull(genericTypeArgument, nameof(genericTypeArgument));

            this.genericTypeArguments.Add(genericTypeArgument);
        }
    }
}