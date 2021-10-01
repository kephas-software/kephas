// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamic.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDynamic interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    /// <summary>
    /// Makes an object dynamic by accessing its members by their name.
    /// </summary>
    public interface IDynamic
    {
        /// <summary>
        /// Convenience method that provides a string Indexer
        /// to the Members collection AND the strongly typed
        /// members of the object by name.
        /// // dynamic
        /// exp["Address"] = "112 nowhere lane";
        /// // strong
        /// var name = exp["StronglyTypedProperty"] as string;.
        /// </summary>
        /// <value>
        /// The <see cref="object" /> identified by the key.
        /// </value>
        /// <param name="key">The key identifying the member name.</param>
        /// <returns>The requested member value.</returns>
        object? this[string key] { get; set; }
    }
}