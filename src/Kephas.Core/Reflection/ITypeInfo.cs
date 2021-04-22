// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract providing type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// Contract providing type information.
    /// </summary>
    public interface ITypeInfo : IElementInfo, IPrototype
    {
        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        string Namespace { get; }

        /// <summary>
        /// Gets the full name qualified with the module where it was defined.
        /// </summary>
        /// <value>
        /// The full name qualified with the module.
        /// </value>
        string QualifiedFullName { get; }

        /// <summary>
        /// Gets the bases of this <see cref="ITypeInfo"/>. They include the real base and also the implemented interfaces.
        /// </summary>
        /// <value>
        /// The bases.
        /// </value>
        IEnumerable<ITypeInfo> BaseTypes { get; }

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type parameters of a generic type definition (open generic).
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        IReadOnlyList<ITypeInfo> GenericTypeParameters { get; }

        /// <summary>
        /// Gets a read-only list of <see cref="ITypeInfo"/> objects that represent the type arguments of a closed generic type.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        IReadOnlyList<ITypeInfo> GenericTypeArguments { get; }

        /// <summary>
        /// Gets a <see cref="ITypeInfo"/> object that represents a generic type definition from which the current generic type can be constructed.
        /// </summary>
        /// <value>
        /// The generic type definition.
        /// </value>
        ITypeInfo? GenericTypeDefinition { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        IEnumerable<IPropertyInfo> Properties { get; }

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <value>
        /// The members.
        /// </value>
        IEnumerable<IElementInfo> Members { get; }

        /// <summary>
        /// Gets the container type registry.
        /// </summary>
        ITypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets a member by the provided name.
        /// </summary>
        /// <param name="name">The member name.</param>
        /// <param name="throwIfNotFound">True to throw if the requested member is not found.</param>
        /// <returns>
        /// The requested member, or <c>null</c>.
        /// </returns>
        IElementInfo? GetMember(string name, bool throwIfNotFound = true);

        /// <summary>
        /// Constructs a generic type based on the provided type arguments.
        /// </summary>
        /// <param name="typeArguments">The type arguments.</param>
        /// <param name="constructionContext">Optional. The construction context.</param>
        /// <returns>
        /// A constructed <see cref="ITypeInfo"/>.
        /// </returns>
        ITypeInfo MakeGenericType(IEnumerable<ITypeInfo> typeArguments, IContext? constructionContext = null);

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Gets the <see cref="Type"/> for the provided <see cref="ITypeInfo"/> instance.
        /// </summary>
        /// <returns>
        /// The provided <see cref="ITypeInfo"/>'s associated <see cref="Type"/>.
        /// </returns>
        Type AsType()
        {
            // TODO optimize
            Type? type = null;
            if (this is IRuntimeTypeInfo runtimeEntityType)
            {
                type = runtimeEntityType.Type;
            }
            else if (this is IAggregatedElementInfo aggregate)
            {
                type = aggregate.Parts.OfType<Type>().FirstOrDefault();
                if (type == null)
                {
                    type = aggregate.Parts.OfType<IRuntimeTypeInfo>().FirstOrDefault()?.Type;
                }
            }

            if (type == null)
            {
                // TODO localization
                throw new InvalidCastException($"No type could be identified for {this}.");
            }

            return type;
        }

        /// <summary>
        /// Indicates whether the <see cref="ITypeInfo"/> is a generic type.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the type is generic, either closed or open; <c>false</c> if not.
        /// </returns>
        bool IsGenericType()
        {
            return this.GenericTypeArguments?.Count > 0 || this.GenericTypeParameters?.Count > 0;
        }

        /// <summary>
        /// Indicates whether the <see cref="ITypeInfo"/> is a generic type definition (aka open generic).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the type is a generic type definition; <c>false</c> if not.
        /// </returns>
        bool IsGenericTypeDefinition()
        {
            return this.GenericTypeParameters?.Count > 0 && this.GenericTypeDefinition == null;
        }

        /// <summary>
        /// Indicates whether the <see cref="ITypeInfo"/> is a generic type definition (aka open generic).
        /// </summary>
        /// <returns>
        /// <c>true</c> if the type is a generic type definition; <c>false</c> if not.
        /// </returns>
        bool IsConstructedGenericType()
        {
            return this.GenericTypeArguments?.Count > 0 && this.GenericTypeDefinition != null;
        }

        /// <summary>
        /// Gets the model element's own members, excluding those declared by the base element or mixins.
        /// </summary>
        /// <returns>The members declared exclusively at the type level.</returns>
        IEnumerable<IElementInfo> GetDeclaredMembers()
        {
            return this.Members.Where(m => m.DeclaringContainer == this);
        }
#endif
    }
}