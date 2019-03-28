// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeTypeInfoFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeTypeInfoFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;

    /// <summary>
    /// Contract for factories creating <see cref="IRuntimeTypeInfo"/> instances.
    /// </summary>
    public interface IRuntimeTypeInfoFactory
    {
        /// <summary>
        /// Tries to create the runtime type information type for the provided raw type.
        /// </summary>
        /// <param name="type">The raw type.</param>
        /// <returns>
        /// The matching runtime type information type, or <c>null</c> if a runtime type info could not be created.
        /// </returns>
        IRuntimeTypeInfo TryCreateRuntimeTypeInfo(Type type);
    }
}