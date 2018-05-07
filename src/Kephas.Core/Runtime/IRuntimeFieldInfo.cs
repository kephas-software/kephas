// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeFieldInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeFieldInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for runtime field information.
    /// </summary>
    public interface IRuntimeFieldInfo : IFieldInfo, IRuntimeElementInfo
    {
        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <value>
        /// The type of the field.
        /// </value>
        new IRuntimeTypeInfo FieldType { get; }

        /// <summary>
        /// Gets the field information.
        /// </summary>
        /// <value>
        /// The field information.
        /// </value>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets a value indicating whether this field is static.
        /// </summary>
        /// <value>
        /// True if this field is static, false if not.
        /// </value>
        bool IsStatic { get; }
    }
}