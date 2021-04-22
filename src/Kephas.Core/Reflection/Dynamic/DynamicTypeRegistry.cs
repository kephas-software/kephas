// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Runtime;

    /// <summary>
    /// A type registry for dynamic types.
    /// </summary>
    public class DynamicTypeRegistry : Expando, ITypeRegistry, IElementInfo
    {
        private readonly string name;
        private readonly string fullName;
        private readonly DynamicElementInfoCollection<ITypeInfo> types;
        private readonly IList<object> annotations = new List<object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeRegistry"/> class.
        /// </summary>
        public DynamicTypeRegistry()
            : this(nameof(DynamicTypeRegistry), nameof(DynamicTypeRegistry))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeRegistry"/> class.
        /// </summary>
        /// <param name="name">The type registry name.</param>
        /// <param name="fullName">The type registry full name.</param>
        /// <param name="annotations">Optional. The annotations.</param>
        protected DynamicTypeRegistry(string name, string fullName, IEnumerable<object>? annotations = null)
        {
            this.name = name;
            this.fullName = fullName;
            this.types = new (this);
            if (annotations != null)
            {
                this.annotations.AddRange(annotations);
            }
        }

        /// <summary>
        /// Gets the collection of types.
        /// </summary>
        public ICollection<ITypeInfo> Types => this.types;

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <value>
        /// The name of the element.
        /// </value>
        string IElementInfo.Name => this.name;

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        string IElementInfo.FullName => this.fullName;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => this.annotations;

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        IElementInfo? IElementInfo.DeclaringContainer => null;

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public IDisplayInfo? GetDisplayInfo() => null;

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        IEnumerable<TAttribute> IAttributeProvider.GetAttributes<TAttribute>() => this.annotations.OfType<TAttribute>();

        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <returns>The type information.</returns>
        public virtual ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true)
        {
            var typeInfo = typeToken switch
            {
                Guid id => this.types.FirstOrDefault(t => id.Equals((t as IIdentifiable)?.Id)),
                string name => this.types.FirstOrDefault(t => t.FullName == name)
                               ?? this.types.FirstOrDefault(t => t.Name == name),
                _ => null,
            };

            if (typeInfo == null && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Type with token '{typeToken}' not found.");
            }

            return typeInfo;
        }
    }
}