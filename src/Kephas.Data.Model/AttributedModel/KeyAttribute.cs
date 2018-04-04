// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the key attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.AttributedModel
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Defines a key for the annotated entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = true)]
    public class KeyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="keyProperties">The key properties.</param>
        public KeyAttribute(string[] keyProperties)
            : this(null, KeyKind.Default, keyProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="keyProperties">The key properties.</param>
        public KeyAttribute(string name, string[] keyProperties)
            : this(name, KeyKind.Default, keyProperties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
        /// </summary>
        /// <param name="name">The key name.</param>
        /// <param name="kind">The key kind.</param>
        /// <param name="keyProperties">The key properties.</param>
        protected KeyAttribute(string name, KeyKind kind, string[] keyProperties)
        {
            Requires.NotNullOrEmpty(keyProperties, nameof(keyProperties));

            this.Kind = kind;
            this.Name = name;
            this.KeyProperties = keyProperties;
        }

        /// <summary>
        /// Gets the key name. If not provided, a generated name will be used.
        /// </summary>
        /// <value>
        /// The key name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the key properties.
        /// </summary>
        /// <value>
        /// The key properties.
        /// </value>
        public string[] KeyProperties { get; }

        /// <summary>
        /// Gets the key kind.
        /// </summary>
        /// <value>
        /// The key kind.
        /// </value>
        public KeyKind Kind { get; }
    }
}