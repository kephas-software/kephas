// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Kephas.Activation;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="Type"/>, <see cref="TypeInfo"/>, and <see cref="ITypeInfo"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The name of the extended model type info property.
        /// </summary>
        private static readonly string ModelTypeInfoName = "Kephas_" + nameof(ModelTypeInfoName);

        /// <summary>
        /// A Type extension method that converts a type to a model type information.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public static ITypeInfo AsModelTypeInfo(this Type type)
        {
            Requires.NotNull(type, nameof(type));

            return type.AsRuntimeTypeInfo().AsModelTypeInfo();
        }

        /// <summary>
        /// A TypeInfo extension method that converts a type to a model type information.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public static ITypeInfo AsModelTypeInfo(this TypeInfo type)
        {
            Requires.NotNull(type, nameof(type));

            return type.AsRuntimeTypeInfo().AsModelTypeInfo();
        }

        /// <summary>
        /// A <see cref="ITypeInfo"/> extension method that converts a type to a model type information.
        /// </summary>
        /// <param name="type">The type to act on.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public static ITypeInfo AsModelTypeInfo(this ITypeInfo type)
        {
            Requires.NotNull(type, nameof(type));

            if (!(type[ModelTypeInfoName] is ITypeInfo modelType))
            {
                var entityFor = type.GetAttribute<ImplementationForAttribute>();
                modelType = (entityFor != null ? entityFor.AbstractType?.AsRuntimeTypeInfo() : type) ?? type;
                type[ModelTypeInfoName] = modelType;
            }

            return modelType;
        }

        /// <summary>
        /// An object extension method that gets model type information.
        /// </summary>
        /// <param name="obj">The obj to act on.</param>
        /// <returns>
        /// The model type information.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypeInfo GetModelTypeInfo(this object obj)
        {
            return obj?.GetType().AsModelTypeInfo();
        }
    }
}