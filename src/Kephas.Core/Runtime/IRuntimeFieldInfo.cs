// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeFieldInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    }
}