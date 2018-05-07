// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IParameterInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Interface for parameter information.
    /// </summary>
    public interface IParameterInfo : IElementInfo
    {
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        /// <value>
        /// The type of the parameter.
        /// </value>
        ITypeInfo ParameterType { get; }
    }
}