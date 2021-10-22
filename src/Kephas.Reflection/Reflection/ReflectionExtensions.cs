// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Reflection;

    using Kephas.Runtime;

    /// <summary>
    /// Extension methods used in reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets the most specific type information out of the provided instance.
        /// If the object implements <see cref="IInstance"/>, then it returns
        /// the <see cref="ITypeInfo"/> provided by it, otherwise it returns the <see cref="IRuntimeTypeInfo"/>
        /// of its runtime type.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A type information for the provided object.</returns>
        public static ITypeInfo GetTypeInfo(this object obj)
        {
            obj = obj ?? throw new ArgumentNullException(nameof(obj));

            var typeInfo = (obj as IInstance)?.GetTypeInfo();
            return typeInfo ?? obj.GetType().AsRuntimeTypeInfo();
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