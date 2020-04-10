// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverSerializationBinder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type resolver serialization binder class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Json
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;

    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A type resolver serialization binder.
    /// </summary>
    public class TypeResolverSerializationBinder : DefaultSerializationBinder
    {
        /// <summary>
        /// The type resolver.
        /// </summary>
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolverSerializationBinder"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        public TypeResolverSerializationBinder(ITypeResolver typeResolver)
        {
            Requires.NotNull(typeResolver, nameof(typeResolver));

            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Resolves the type with the provided assembly name and type name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            var qualifiedTypeName = string.IsNullOrEmpty(assemblyName)
                                        ? typeName
                                        : $"{typeName}, {assemblyName}";
            return this.typeResolver.ResolveType(qualifiedTypeName, throwOnNotFound: false)
                   ?? base.BindToType(assemblyName, typeName);
        }

        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        /// <param name="serializedType">The type of the object the formatter creates a new instance of.</param>
        /// <param name="assemblyName">[out] Specifies the <see cref="T:System.Reflection.Assembly" />
        ///                            name of the serialized object.</param>
        /// <param name="typeName">[out] Specifies the <see cref="T:System.Type" /> name of the
        ///                        serialized object.</param>
        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            base.BindToName(serializedType, out _, out typeName);
            assemblyName = null;
        }
    }
}