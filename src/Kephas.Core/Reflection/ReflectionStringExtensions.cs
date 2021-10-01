// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionStringExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the reflection string extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;

    using Kephas.Runtime;

    /// <summary>
    /// String extensions for reflection purposes.
    /// </summary>
    public static class ReflectionStringExtensions
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
    }
}