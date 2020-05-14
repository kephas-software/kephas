// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeResolver.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Composition.AttributedModel;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// A default service implementation of the <see cref="ITypeResolver"/> service contract.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultTypeResolver : Loggable, ITypeResolver
    {
        private readonly Func<IEnumerable<Assembly>> getAppAssemblies;
        private readonly ConcurrentDictionary<string, Type?> typeCache = new ConcurrentDictionary<string, Type?>();
        private readonly ITypeLoader typeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeResolver"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        [CompositionConstructor]
        public DefaultTypeResolver(IAppRuntime appRuntime, ITypeLoader? typeLoader = null, ILogManager? logManager = null)
            : base(logManager)
        {
            Requires.NotNull(appRuntime, nameof(appRuntime));

            this.typeLoader = typeLoader ?? new DefaultTypeLoader(logManager);
            this.getAppAssemblies = () => appRuntime.GetAppAssemblies();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeResolver"/> class.
        /// </summary>
        /// <param name="getAppAssemblies">The get application assemblies.</param>
        /// <param name="typeLoader">Optional. The type loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public DefaultTypeResolver(Func<IEnumerable<Assembly>> getAppAssemblies, ITypeLoader? typeLoader = null, ILogManager logManager = null)
            : base(logManager)
        {
            Requires.NotNull(getAppAssemblies, nameof(getAppAssemblies));

            this.typeLoader = typeLoader ?? new DefaultTypeLoader(logManager);
            this.getAppAssemblies = getAppAssemblies;
        }

        /// <summary>
        /// Resolves a type based on the provided type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="throwOnNotFound">Indicates whether to throw or not when the indicated type is not found.</param>
        /// <returns>
        /// A Type.
        /// </returns>
        public Type? ResolveType(string typeName, bool throwOnNotFound = true)
        {
            var type = this.typeCache.GetOrAdd(typeName, _ => this.ResolveTypeCore(typeName));
            if (type == null && throwOnNotFound)
            {
                throw new TypeLoadException(string.Format(Strings.DefaultTypeResolver_ResolveType_NotFound_Exception, typeName));
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
        protected virtual Type? ResolveTypeCore(string typeName)
        {
            try
            {
                var type = Type.GetType(typeName, throwOnError: false);
                if (type != null)
                {
                    return type;
                }

                var qualifiedName = new QualifiedFullName(typeName);
                if (qualifiedName.AssemblyName == null)
                {
                    type = this.getAppAssemblies()
                        .Select(asm => asm.GetType(qualifiedName.TypeName, throwOnError: false))
                        .FirstOrDefault(t => t != null);

                    if (type == null && qualifiedName.Namespace == null)
                    {
                        type = this.ResolveTypeByNameOnly(qualifiedName.Name, this.getAppAssemblies());
                    }

                    return type;
                }

                var assemblyName = qualifiedName.AssemblyName.Name;
                var assembly = this.getAppAssemblies()
                    .FirstOrDefault(a => a.GetName().Name == assemblyName);
                if (assembly == null)
                {
                    throw new TypeLoadException($"Assembly '{assemblyName}' not found.");
                }

                type = assembly.GetType(qualifiedName.TypeName, throwOnError: false);
                if (type == null && qualifiedName.Namespace == null)
                {
                    type = this.ResolveTypeByNameOnly(qualifiedName.Name, new[] { assembly });
                }

                return type;
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, Strings.DefaultTypeResolver_ResolveTypeCore_Exception, typeName);
                return null;
            }
        }

        private Type? ResolveTypeByNameOnly(string name, IEnumerable<Assembly> assemblies)
        {
            var matchingTypes = assemblies
                .SelectMany(asm => this.typeLoader.GetExportedTypes(asm).Where(t => t.Name == name))
                .Take(2)
                .ToList();
            if (matchingTypes.Count > 1)
            {
                throw new AmbiguousMatchException(
                    $"Multiple types with the name '{name}' found, at least '{matchingTypes[0]}' and '{matchingTypes[1]}'.");
            }

            return matchingTypes.Count == 0 ? null : matchingTypes[0];
        }
    }
}