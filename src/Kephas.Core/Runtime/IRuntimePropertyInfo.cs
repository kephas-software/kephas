// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimePropertyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for dynamically accessing a property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Contract for a dynamic <see cref="PropertyInfo"/>.
    /// </summary>
    public interface IRuntimePropertyInfo : IPropertyInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        /// <value>
        /// The type of the property.
        /// </value>
        new IRuntimeTypeInfo PropertyType { get; }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this property is static.
        /// </summary>
        /// <value>
        /// True if this property is static, false if not.
        /// </value>
        bool IsStatic { get; }
    }
}
