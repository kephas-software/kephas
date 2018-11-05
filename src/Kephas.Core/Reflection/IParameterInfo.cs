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
    public interface IParameterInfo : IValueElementInfo
    {
        /// <summary>
        /// Gets the position in the parameter's list.
        /// </summary>
        /// <value>
        /// The position in the parameter's list.
        /// </value>
        int Position { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the parameter is optional, <c>false</c> otherwise.
        /// </value>
        bool IsOptional { get; }
    }
}