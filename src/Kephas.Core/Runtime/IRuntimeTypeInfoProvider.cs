// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeInfoProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeTypeInfoProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;

    /// <summary>
    /// Interface for runtime type information provider.
    /// </summary>
    public interface IRuntimeTypeInfoProvider
    {
        /// <summary>
        /// Gets the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type.
        /// </returns>
        Type GetRuntimeTypeInfoType(Type type);
    }
}