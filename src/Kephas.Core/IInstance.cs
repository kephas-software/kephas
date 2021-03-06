// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstance.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for instances of classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using Kephas.Reflection;

    /// <summary>
    /// Contract for instances of classifiers.
    /// </summary>
    public interface IInstance
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
#if NETSTANDARD2_0
        ITypeInfo GetTypeInfo();
#else
        ITypeInfo GetTypeInfo() => this.GetType().AsRuntimeTypeInfo();
#endif
    }

    /// <summary>
    /// Generic contract for instances of classifiers.
    /// </summary>
    /// <typeparam name="T">The specific type info.</typeparam>
    public interface IInstance<out T> : IInstance
        where T : ITypeInfo
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
#if NETSTANDARD2_0
        new T GetTypeInfo();
#else
        new T GetTypeInfo() => (T)((IInstance)this).GetTypeInfo();
#endif
    }
}