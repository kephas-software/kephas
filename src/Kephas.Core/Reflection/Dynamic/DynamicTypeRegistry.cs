// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Data.Formatting;
    using Kephas.Dynamic;
    using Kephas.Runtime;
    using Kephas.Threading.Tasks;

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
                Type type => this.runtimeTypeRegistry.GetTypeInfo(type, throwOnNotFound),
                ITypeInfo ti => ti,
                _ => null,
            };

            if (typeInfo == null && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Type with token '{typeToken}' not found.");
            }

            return typeInfo;
        }

        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>The type information.</returns>
        public virtual async Task<ITypeInfo?> GetTypeInfoAsync(object typeToken, bool throwOnNotFound = true, CancellationToken cancellationToken = default)
        {
            var typeInfo = typeToken switch
            {
                Guid id => this.types.FirstOrDefault(t => id.Equals((t as IIdentifiable)?.Id)),
                string name => this.types.FirstOrDefault(t => t.FullName == name)
                               ?? this.types.FirstOrDefault(t => t.Name == name)
                               ?? await this.ResolveTypeInfoAsync(name, throwOnNotFound, cancellationToken).PreserveThreadContext(),
                Type type => await this.runtimeTypeRegistry.GetTypeInfoAsync(type, throwOnNotFound, cancellationToken).PreserveThreadContext(),
                _ => null,
            };

            if (typeInfo == null && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Type with token '{typeToken}' not found.");
            }

            return typeInfo;
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<ITypeInfo> GetEnumerator() => this.types.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.types.GetEnumerator();

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

        /// <summary>
        /// Resolves the <see cref="ITypeInfo"/> based on the provided type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found, throws an exception.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The type information.</returns>
        protected virtual async Task<ITypeInfo?> ResolveTypeInfoAsync(string typeName, bool throwOnNotFound, CancellationToken cancellationToken)
        {
            if (this.typeResolver == null)
            {
                return throwOnNotFound
                    ? throw new KeyNotFoundException($"Type with name '{typeName}' not found. Try to provide a type resolver for resolving type names.")
                    : null;
            }

            var type = this.typeResolver.ResolveType(typeName, throwOnNotFound);
            return type == null ? null : await this.runtimeTypeRegistry.GetTypeInfoAsync(type, throwOnNotFound, cancellationToken).PreserveThreadContext();
        }
    }
}