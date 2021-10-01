// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPrototype.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides the <see cref="CreateInstance"/> method for creating objects based on this prototype.
    /// </summary>
    public interface IPrototype
    {
        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">Optional. The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        object CreateInstance(IEnumerable<object?>? args = null);
    }
}