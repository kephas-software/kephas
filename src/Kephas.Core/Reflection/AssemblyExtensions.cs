// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the assembly extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Resources;

    /// <summary>
    /// Extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to act on.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        public static IEnumerable<Type> GetLoadableExportedTypes(this Assembly assembly)
        {
            // for more information check also:
            // http://stackoverflow.com/questions/7889228/how-to-prevent-reflectiontypeloadexception-when-calling-assembly-gettypes
            Contract.Requires(assembly != null);

            try
            {
                return assembly.ExportedTypes;
            }
            catch (ReflectionTypeLoadException e)
            {
                var logger = GetLogger();
                logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
            catch (TypeLoadException tle)
            {
                var logger = GetLogger();
                logger.Error(tle, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_TypeLoadException, tle.TypeName, assembly.FullName));

                return GetLoadableDefinedTypes(assembly);
            }
            catch (Exception e)
            {
                var logger = GetLogger();
                logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));

                return GetLoadableDefinedTypes(assembly);
            }
        }

        /// <summary>
        /// Gets the loadable defined types in the assembly.
        /// </summary>
        /// <param name="assembly">The assembly to act on.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        private static IEnumerable<Type> GetLoadableDefinedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes.Where(t => t.IsPublic).Select(t => t.AsType());
            }
            catch (ReflectionTypeLoadException e)
            {
                var logger = GetLogger();
                logger.Error(e, string.Format(Strings.AssemblyExtensions_GetLoadableDefinedTypes_ReflectionTypeLoadException, assembly.FullName));
                return e.Types.Where(t => t != null);
            }
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns>
        /// The logger.
        /// </returns>
        private static ILogger GetLogger()
        {
            var logger = AmbientServices.Instance.LogManager.GetLogger(typeof(AssemblyExtensions).FullName);
            return logger;
        }
    }
}