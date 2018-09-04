// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITypeLoader interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Application service contract for loading types.
    /// </summary>
    public interface ITypeLoader
    {
        /// <summary>
        /// Gets the loadable exported types from the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the types.</param>
        /// <returns>
        /// An enumeration of types.
        /// </returns>
        IEnumerable<Type> GetLoadableExportedTypes(Assembly assembly);
    }
}