// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

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
        IRuntimeTypeInfo GetTypeInfo([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type);

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
}