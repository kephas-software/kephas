// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeElementInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for dynamic element information.
    /// </summary>
    public interface IRuntimeElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the underlying element information.
        /// </summary>
        /// <returns>
        /// The underlying element information.
        /// </returns>
        ICustomAttributeProvider GetUnderlyingElementInfo();
    }
}