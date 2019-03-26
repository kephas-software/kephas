// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeInfoTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime type information attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute indicating the type of runtime type information to use for a specific type. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    public sealed class RuntimeTypeInfoTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeTypeInfoTypeAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// The provided type must implement <see cref="IRuntimeTypeInfo"/>,
        /// and inherits typically from <see cref="RuntimeTypeInfo"/>.
        /// </remarks>
        /// <param name="type">The type of the runtime type information.</param>
        public RuntimeTypeInfoTypeAttribute(Type type)
        {
            Requires.NotNull(type, nameof(type));

            this.Type = type;
        }

        /// <summary>
        /// Gets the type of the runtime type information.
        /// </summary>
        /// <value>
        /// The type of the runtime type information.
        /// </value>
        public Type Type { get; }
    }
}