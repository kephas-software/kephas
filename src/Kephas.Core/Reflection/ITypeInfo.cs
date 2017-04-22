// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract providing type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract providing type information.
    /// </summary>
    public interface ITypeInfo : IElementInfo
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
        ITypeInfo GenericTypeDefinition { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        IEnumerable<IPropertyInfo> Properties { get; }
    }
}