// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpandoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the expando extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.ServiceStack.Text
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Extension methods for <see cref="IExpando"/>.
    /// </summary>
    internal static class ExpandoExtensions
    {
        /// <summary>
        /// Converts an expando to a dictionary on all its depth.
        /// </summary>
        /// <param name="expando">The expando.</param>
        /// <returns>
        /// Expando as an IDictionary&lt;string,object&gt;.
        /// </returns>
        public static IDictionary<string, object> ToDictionaryDeep(this IExpando expando)
        {
            return expando.ToDictionary(valueFunc: v => v is IExpando expandoValue ? ToDictionaryDeep(expandoValue) : v);
        }
    }
}