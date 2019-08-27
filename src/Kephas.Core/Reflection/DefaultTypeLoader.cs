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

    /// <summary>
    /// The default implementation of <see cref="ITypeLoader"/>.
    /// </summary>
    public class DefaultTypeLoader : Loggable, ITypeLoader
    {
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
                this.Logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
            catch (TypeLoadException tle)
            {
                this.Logger.Error(tle, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_TypeLoadException, tle.TypeName, assembly.FullName));

                return this.GetLoadableDefinedTypes(assembly);
            }
            catch (Exception e)
            {
                this.Logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));

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
                this.Logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableDefinedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
        }
    }
}