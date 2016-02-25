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
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}