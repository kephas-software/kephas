// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Helper class for the runtime area.
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// Gets the type's proper properties: public, non-static, and without parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An enumeration of property infos.</returns>
        public static IEnumerable<PropertyInfo> GetTypeProperties(Type type)
        {
            return type.GetRuntimeProperties()
                .Where(p => p.GetMethod != null && !p.GetMethod.IsStatic && p.GetMethod.IsPublic
                            && p.GetIndexParameters().Length == 0);
        }
    }
}