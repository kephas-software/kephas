// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IElementInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract providing base element information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;

    using Kephas.Dynamic;
    using Kephas.Runtime;

    /// <summary>
    /// Contract providing base element information.
    /// </summary>
    public interface IElementInfo : IExpando, IAttributeProvider, INamed
    {
        /// <summary>
        /// Gets the full name of the element.
        /// </summary>
        /// <value>
        /// The full name of the element.
        /// </value>
        string FullName { get; }

        /// <summary>
        /// Gets the element annotations.
        /// </summary>
        /// <value>
        /// The element annotations.
        /// </value>
        IEnumerable<object> Annotations { get; }

        /// <summary>
        /// Gets the parent element declaring this element.
        /// </summary>
        /// <value>
        /// The declaring element.
        /// </value>
        IElementInfo? DeclaringContainer { get; }

#if NETSTANDARD2_1
        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        IDisplayInfo? GetDisplayInfo() => ElementInfoHelper.GetDisplayInfo(this);
#else
        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        IDisplayInfo? GetDisplayInfo();
#endif
    }
}