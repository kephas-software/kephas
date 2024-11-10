// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
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
    public interface IParameterInfo : IValueElementInfo, IPositionalElementInfo
    {
        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        bool IsOptional { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter is for input.
        /// </summary>
        /// <value>
        /// True if this parameter is for input, false if not.
        /// </value>
        bool IsIn { get; }

        /// <summary>
        /// Gets a value indicating whether the parameter is for output.
        /// </summary>
        /// <value>
        /// True if this parameter is for output, false if not.
        /// </value>
        bool IsOut { get; }
    }
}