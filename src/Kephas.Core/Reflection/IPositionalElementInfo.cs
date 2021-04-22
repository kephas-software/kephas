// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPositionalElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Contract for reflection elements contained in an ordered container.
    /// </summary>
    public interface IPositionalElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the position in the declaring container.
        /// </summary>
        /// <value>
        /// The position in the declaring container.
        /// </value>
        int Position { get; }
    }
}