// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataFormattable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Formatting
{
    /// <summary>
    /// Extracts the information contained in this object to a serialization friendly representation.
    /// </summary>
    public interface IDataFormattable
    {
        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        object ToData(IDataFormattingContext? context = null)
#if NETSTANDARD2_0
        ;
#else
        {
            return this.ToString();
        }
#endif
    }
}