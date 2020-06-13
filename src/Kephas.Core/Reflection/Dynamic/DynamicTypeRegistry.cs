// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// A type registry for dynamic types.
    /// </summary>
    public class DynamicTypeRegistry : Expando, ITypeRegistry
    {
        private readonly ConcurrentDictionary<object, ITypeInfo> types = new ConcurrentDictionary<object, ITypeInfo>();

        /// <summary>
        /// Gets a type registry that does nothing.
        /// </summary>
        public static DynamicTypeRegistry Null { get; } = new NullDynamicTypeRegistry();

        /// <summary>
        /// Adds the type information to the registry.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>This registry.</returns>
        public virtual DynamicTypeRegistry AddTypeInfo(DynamicTypeInfo typeInfo)
        {
            this.types.TryAdd(typeInfo.Id, typeInfo);
            return this;
        }

        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <returns>The type information.</returns>
        public virtual ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true)
        {
            if (!this.types.TryGetValue(typeToken, out var typeInfo) && throwOnNotFound)
            {
                throw new KeyNotFoundException($"Type with token '{typeToken}' not found.");
            }

            return typeInfo;
        }

        private class NullDynamicTypeRegistry : DynamicTypeRegistry
        {
            public override DynamicTypeRegistry AddTypeInfo(DynamicTypeInfo typeInfo) => this;

            public override ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true) => null;
        }
    }
}