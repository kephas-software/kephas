// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Resources;

    /// <summary>
    /// The default type loader.
    /// </summary>
    public class DefaultTypeLoader : Loggable, ITypeLoader
    {
        private static readonly Lazy<ITypeLoader> LazyInstance = new Lazy<ITypeLoader>(() => new DefaultTypeLoader());
        private readonly ConcurrentDictionary<Assembly, IEnumerable<Type>> assemblyCache =
            new ConcurrentDictionary<Assembly, IEnumerable<Type>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeLoader"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public DefaultTypeLoader(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Gets the static instance of the <see cref="DefaultTypeLoader"/>.
        /// </summary>
        public static ITypeLoader Instance => LazyInstance.Value;

        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        public IEnumerable<Type> GetExportedTypes(Assembly assembly)
        {
            Requires.NotNull(assembly, nameof(assembly));

            return this.assemblyCache.GetOrAdd(
                assembly,
                _ =>
                {
                    // for more information check also:
                    // http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
                    try
                    {
                        return assembly.IsDynamic ? new Type[0] : assembly.ExportedTypes;
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        this.Logger.Error(e, Strings.DefaultTypeLoader_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly);
                        return e.Types.Where(t => t != null);
                    }
                    catch (TypeLoadException tle)
                    {
                        this.Logger.Error(tle, Strings.DefaultTypeLoader_GetLoadableExportedTypes_TypeLoadException, tle.TypeName, assembly);

                        return this.GetLoadableDefinedTypes(assembly);
                    }
                    catch (Exception e)
                    {
                        this.Logger.Error(e, Strings.DefaultTypeLoader_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly);

                        return this.GetLoadableDefinedTypes(assembly);
                    }
                });
        }

        /// <summary>
        /// Gets the loadable defined types in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to act on.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        private IEnumerable<Type> GetLoadableDefinedTypes(Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes.Where(t => t.IsPublic).Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException e)
            {
                this.Logger.Error(e, Strings.DefaultTypeLoader_GetLoadableDefinedTypes_ReflectionTypeLoadException, assembly);
                return e.Types.Where(t => t != null);
            }
        }
    }
}