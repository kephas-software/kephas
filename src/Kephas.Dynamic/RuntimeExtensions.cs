// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using System;
    using System.Reflection;

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for runtime classes and interfaces.
    /// </summary>
    public static class RuntimeExtensions
    {
        /// <summary>
        /// Gets the <see cref="IRuntimeTypeInfo"/> for the provided <see cref="Type"/> instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The provided <see cref="Type"/>'s associated <see cref="IRuntimeTypeInfo"/>.
        /// </returns>
        public static IRuntimeTypeInfo AsRuntimeTypeInfo(this Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return RuntimeTypeRegistry.Instance.GetTypeInfo(type);
        }

        /// <summary>
        /// Gets a runtime type information out of the provided instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A dynamic type information for the provided object.</returns>
        public static IRuntimeTypeInfo GetRuntimeTypeInfo(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            return obj.GetType().AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeAssemblyInfo"/> for the provided <see cref="Assembly"/> instance.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The provided <see cref="Assembly"/>'s associated <see cref="IRuntimeAssemblyInfo"/>.
        /// </returns>
        public static IRuntimeAssemblyInfo AsRuntimeAssemblyInfo(this Assembly assembly)
        {
            assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            return RuntimeTypeRegistry.Instance.GetAssemblyInfo(assembly);
        }
    }
}