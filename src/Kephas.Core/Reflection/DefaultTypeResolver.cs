// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeResolver.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default type resolver class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A default service implementation of the <see cref="ITypeResolver"/> service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultTypeResolver : ITypeResolver
    {
        /// <summary>
        /// The assembly loader.
        /// </summary>
        private readonly IAssemblyLoader assemblyLoader;

        /// <summary>
        /// The type cache.
        /// </summary>
        private readonly ConcurrentDictionary<string, Type> typeCache = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeResolver"/> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        public DefaultTypeResolver(IAssemblyLoader assemblyLoader)
        {
            Requires.NotNull(assemblyLoader, nameof(assemblyLoader));

            this.assemblyLoader = assemblyLoader;
        }

        /// <summary>
        /// Resolves a type based on the provided type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        public Type ResolveType(string typeName, bool throwOnNotFound = true)
        {
            var type = this.typeCache.GetOrAdd(typeName, _ => this.ResolveTypeCore(typeName));
            if (type == null && throwOnNotFound)
            {
                throw new KeyNotFoundException(string.Format(Strings.DefaultTypeResolver_ResolveType_NotFound_Exception, typeName));
            }

            return type;
        }

        /// <summary>
        /// Core implementation of the <see cref="ResolveType"/> operation.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        protected Type ResolveTypeCore(string typeName)
        {
            var type = Type.GetType(typeName, throwOnError: false);
            if (type != null)
            {
                return type;
            }

            var qualifiedName = new QualifiedFullName(typeName);

            if (qualifiedName.AssemblyName == null)
            {
                return null;
            }

            var assembly = this.assemblyLoader.LoadAssembly(qualifiedName.AssemblyName);
            if (assembly == null)
            {
                return null;
            }

            type = assembly.GetType(qualifiedName.TypeName);

            return type;
        }
    }
}