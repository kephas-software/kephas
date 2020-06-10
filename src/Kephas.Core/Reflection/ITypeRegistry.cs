// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using Kephas.Dynamic;

    /// <summary>
    /// Provides access to type information.
    /// </summary>
    public interface ITypeRegistry : IExpando
    {
        /// <summary>
        /// Gets the type information based on the type token.
        /// </summary>
        /// <param name="typeToken">The type token.</param>
        /// <param name="throwOnNotFound">If true and if the type information is not found based on the provided token, throws an exception.</param>
        /// <returns>The type information.</returns>
        ITypeInfo? GetTypeInfo(object typeToken, bool throwOnNotFound = true);
    }
}
