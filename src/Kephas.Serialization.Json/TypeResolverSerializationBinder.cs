// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeResolverSerializationBinder.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
            return this.typeResolver.ResolveType($"{typeName}, {assemblyName}", throwOnNotFound: false)
                   ?? base.BindToType(assemblyName, typeName);
        }
    }
}