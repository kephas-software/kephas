// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplementationForAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client model for attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Activation
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute associating implementations to their abstract model.
    /// </summary>
    /// <remarks>
    /// This attribute is typically used to annotate the concrete implementation of an abstract type
    /// defined typically either as an abstract class or as an interface. This association helps the infrastructure
    /// to find the concrete implementation based only on the abstract type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ImplementationForAttribute : Attribute
    {
        /// <summary>
        /// An empty list of types.
        /// </summary>
        private static readonly Type[] EmptyTypes = new Type[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplementationForAttribute"/> class.
        /// </summary>
        /// <param name="abstractType">The model type associated to the attributed type.</param>
        /// <param name="abstractTypeParts">A variable-length parameters list containing parts of the provided abstract type.</param>
        public ImplementationForAttribute(Type abstractType, params Type[] abstractTypeParts)
        {
            Requires.NotNull(abstractType, nameof(abstractType));

            this.AbstractType = abstractType;
            this.AbstractTypeParts = abstractTypeParts ?? EmptyTypes;
        }

        /// <summary>
        /// Gets the abstract type associated to the attributed type.
        /// </summary>
        /// <value>
        /// The associated abstract type.
        /// </value>
        public Type AbstractType { get; }

        /// <summary>
        /// Gets the parts of the abstract type.
        /// </summary>
        /// <value>
        /// The abstract type parts.
        /// </value>
        public Type[] AbstractTypeParts { get; }
    }
}