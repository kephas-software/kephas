// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract providing property information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Contract providing property information.
    /// </summary>
    public interface IPropertyInfo : IValueElementInfo
    {
        /// <summary>
        /// Gets a value indicating whether the property can be written to.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property can be written to; otherwise <c>false</c>.
        /// </value>
        bool CanWrite { get; }

        /// <summary>
        /// Gets a value indicating whether the property value can be read.
        /// </summary>
        /// <value>
        /// <c>true</c> if the property value can be read; otherwise <c>false</c>.
        /// </value>
        bool CanRead { get; }
    }
}