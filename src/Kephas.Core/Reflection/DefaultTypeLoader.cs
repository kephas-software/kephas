// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default type loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Resources;
    using Kephas.Services;

    /// <summary>
    /// The default implementtion of <see cref="ITypeLoader"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultTypeLoader : ITypeLoader
    {
        /// <summary>
        /// The ambient services.
        /// </summary>
        private readonly IAmbientServices ambientServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTypeLoader"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services (optional).</param>
        public DefaultTypeLoader(IAmbientServices ambientServices = null)
        {
            this.ambientServices = ambientServices;
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger<DefaultTypeLoader> Logger { get; set; }

        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        public IEnumerable<Type> GetLoadableExportedTypes(Assembly assembly)
        {
            // for more information check also:
            // http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
            try
            {
                return assembly.ExportedTypes;
            }
            catch (ReflectionTypeLoadException e)
            {
                this.GetLogger().Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
            catch (TypeLoadException tle)
            {
                this.GetLogger().Error(tle, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_TypeLoadException, tle.TypeName, assembly.FullName));

                return this.GetLoadableDefinedTypes(assembly);
            }
            catch (Exception e)
            {
                this.GetLogger().Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));

                return this.GetLoadableDefinedTypes(assembly);
            }
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
                this.GetLogger().Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableDefinedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns>
        /// The logger.
        /// </returns>
        private ILogger GetLogger()
        {
            // if the Logger property is set, it means it was instantiated by the
            // composition container, otherwise it resided in the ambient services.
            return this.Logger ?? this.ambientServices?.GetLogger<DefaultTypeLoader>();
        }
    }
}