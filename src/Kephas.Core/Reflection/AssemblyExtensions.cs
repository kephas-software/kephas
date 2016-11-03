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
        /// An enumerator that allows foreach to be used to process the loadable exported types in this
        /// collection.
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
            catch (ReflectionTypeLoadException rtlex)
            {
                var logger = GetLogger();
                logger.Error(rtlex, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));

                return rtlex.Types.Where(t => t != null);
            }
            catch (TypeLoadException tlex)
            {
                var logger = GetLogger();
                logger.Error(tlex, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_TypeLoadException, tlex.TypeName, assembly.FullName));

                try
                {
                    return assembly.DefinedTypes.Where(t => t.IsPublic).Select(t => t.AsType());
                }
                catch (ReflectionTypeLoadException rtlex)
                {
                    logger.Error(rtlex, string.Format(Strings.AssemblyExtensions_GetLoadableExportedTypes_ReflectionTypeLoadException, assembly.FullName));
                    return rtlex.Types.Where(t => t != null);
                }
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