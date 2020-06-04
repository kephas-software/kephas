// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Reflection;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The interface for the runtime type serviceRegistry.
    /// </summary>
    public interface IRuntimeTypeRegistry
    {
        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime type.</returns>
        IRuntimeTypeInfo GetRuntimeType(Type type);

#if NETSTANDARD2_1
        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>A runtime type.</returns>
        public IRuntimeTypeInfo GetRuntimeType(TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return this.GetRuntimeType(typeInfo.AsType());
        }
#endif

        /// <summary>
        /// Gets the runtime assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A runtime assembly.</returns>
        IRuntimeAssemblyInfo GetRuntimeAssembly(Assembly assembly);

        /// <summary>
        /// Registers a factory used to create <see cref="IRuntimeTypeInfo"/> instances.
        /// </summary>
        /// <remarks>
        /// Factories are called in the inverse order of their addition, meaning that the last added factory
        /// is invoked first. This is by design, so that the non-framework code has a change to override the
        /// default behavior.
        /// </remarks>
        /// <param name="factory">The factory.</param>
        void RegisterFactory(IRuntimeTypeInfoFactory factory);
    }

#if NETSTANDARD2_1
#else
    /// <summary>
    /// Extension methods for <see cref="IRuntimeTypeRegistry"/>.
    /// </summary>
    public static class RuntimeTypeRegistryExtensions
    {
        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="typeRegistry">The type serviceRegistry.</param>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>A runtime type.</returns>
        public static IRuntimeTypeInfo GetRuntimeType(this IRuntimeTypeRegistry typeRegistry, TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return typeRegistry.GetRuntimeType(typeInfo.AsType());
        }
    }
#endif
}