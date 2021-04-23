﻿// --------------------------------------------------------------------------------------------------------------------
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

    using Kephas.Data;
    using Kephas.Dynamic;
    using Kephas.Runtime;

    /// <summary>
    /// A type registry for dynamic types.
    /// </summary>
    public class DynamicTypeRegistry : Expando, ITypeRegistry, IElementInfo
    {
        private readonly ITypeResolver? typeResolver;
        private readonly IRuntimeTypeRegistry runtimeTypeRegistry;
        private readonly DynamicElementInfoCollection<ITypeInfo> types;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeRegistry"/> class.
        /// </summary>
        /// <param name="runtimeTypeRegistry">Optional. The runtime type registry. If not provided, the <see cref="RuntimeTypeRegistry.Instance"/> is used.</param>
        /// <param name="typeResolver">Optional. The type resolver.</param>
        public DynamicTypeRegistry(IRuntimeTypeRegistry? runtimeTypeRegistry = null, ITypeResolver? typeResolver = null)
        {
            this.typeResolver = typeResolver;
            this.runtimeTypeRegistry = runtimeTypeRegistry ?? RuntimeTypeRegistry.Instance;
            this.types = new (this);
            this.Name = this.GetType().Name;
        }

        /// <summary>
        /// Gets the collection of types.
        /// </summary>
        public ICollection<ITypeInfo> Types => this.types;

        /// <summary>
        /// Gets or sets the name of the registry.
        /// </summary>
        /// <value>
        /// The name of the registry.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        string IElementInfo.FullName => this.Name;

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> IElementInfo.Annotations => Enumerable.Empty<object>();

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
        IDisplayInfo? IElementInfo.GetDisplayInfo() => null;

        /// <summary>
        /// Gets the attribute of the provided type.
        /// </summary>
        /// <typeparam name="TAttribute">Type of the attribute.</typeparam>
        /// <returns>
        /// The attribute of the provided type.
        /// </returns>
        IEnumerable<TAttribute> IAttributeProvider.GetAttributes<TAttribute>() => Enumerable.Empty<TAttribute>();

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
                               ?? this.types.FirstOrDefault(t => t.Name == name)
                               ?? this.ResolveTypeInfo(name, throwOnNotFound),
                _ => null,
            };

            if (typeInfo == null && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Type with token '{typeToken}' not found.");
            }

            return typeInfo;
        }

        /// <summary>
        /// Resolves the <see cref="ITypeInfo"/> based on the provided type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found, throws an exception.</param>
        /// <returns>The type information.</returns>
        protected virtual ITypeInfo? ResolveTypeInfo(string typeName, bool throwOnNotFound)
        {
            if (this.typeResolver == null)
            {
                return throwOnNotFound
                    ? throw new KeyNotFoundException($"Type with name '{typeName}' not found. Try to provide a type resolver for resolving type names.")
                    : null;
            }

            var type = this.typeResolver.ResolveType(typeName, throwOnNotFound);
            return type == null ? null : this.runtimeTypeRegistry.GetTypeInfo(type, throwOnNotFound);
        }
    }
}