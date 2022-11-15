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

        /// <summary>
        /// Indicates whether the <paramref name="memberName"/> is defined in the expando.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// True if defined, false if not.
        /// </returns>
        bool HasDynamicMember(string memberName)
        {
            try
            {
                var _ = this[memberName];
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts the expando to a dictionary having as keys the property names and as values the
        /// respective properties' values.
        /// </summary>
        /// <param name="keyFunc">Optional. The key transformation function.</param>
        /// <param name="valueFunc">Optional. The value transformation function.</param>
        /// <returns>
        /// A dictionary of property values with their associated names.
        /// </returns>
        IDictionary<string, object?> ToDictionary(
            Func<string, string>? keyFunc = null,
            Func<object?, object?>? valueFunc = null) => new Dictionary<string, object?>();
    }
}