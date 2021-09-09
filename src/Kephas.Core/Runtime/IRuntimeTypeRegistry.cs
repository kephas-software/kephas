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
    using Kephas.Reflection;
    using Kephas.Runtime.Factories;

    /// <summary>
    /// The interface for the runtime type serviceRegistry.
    /// </summary>
    public interface IRuntimeTypeRegistry : ITypeRegistry
    {
        /// <summary>
        /// Gets the runtime type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A runtime type.</returns>
        IRuntimeTypeInfo GetTypeInfo(Type type);

        /// <summary>
        /// Gets the runtime type information.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>A runtime type.</returns>
        IRuntimeTypeInfo GetTypeInfo(TypeInfo typeInfo)
        {
            Requires.NotNull(typeInfo, nameof(typeInfo));

            return this.GetTypeInfo(typeInfo.AsType());
        }

        /// <summary>
        /// Gets the runtime assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A runtime assembly.</returns>
        IRuntimeAssemblyInfo GetAssemblyInfo(Assembly assembly);

        /// <summary>
        /// Registers a factory used to create specialized <see cref="IElementInfo"/> instances.
        /// </summary>
        /// <typeparam name="TFactory">The factory type.</typeparam>
        /// <remarks>
        /// Factories are called in the inverse order of their addition, meaning that the last added factory
        /// is invoked first. This is by design, so that the non-framework code has a change to override the
        /// default behavior.
        /// </remarks>
        /// <param name="factory">The factory.</param>
        void RegisterFactory<TFactory>(TFactory factory)
            where TFactory : class, IRuntimeElementInfoFactory;
    }

    /// <summary>
    /// Extension methods for <see cref="IRuntimeTypeRegistry"/>.
    /// </summary>
    public static class RuntimeTypeRegistryExtensions
    {
        /// <summary>
        /// Registers a factory used to create <see cref="IRuntimeTypeInfo"/> instances.
        /// </summary>
        /// <remarks>
        /// Factories are called in the inverse order of their addition, meaning that the last added factory
        /// is invoked first. This is by design, so that the non-framework code has a change to override the
        /// default behavior.
        /// </remarks>
        /// <param name="registry">The type registry.</param>
        /// <param name="factory">The factory.</param>
        public static void RegisterFactory(this IRuntimeTypeRegistry registry, IRuntimeTypeInfoFactory factory)
            => registry.RegisterFactory<IRuntimeTypeInfoFactory>(factory);
    }
}