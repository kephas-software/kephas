// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeKind.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime type kind class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    /// <summary>
    /// Values that represent runtime type kinds.
    /// </summary>
    public enum RuntimeTypeKind
    {
        /// <summary>
        /// The runtime type is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The runtime type is a class.
        /// </summary>
        Class,

        /// <summary>
        /// The runtime type is an interface.
        /// </summary>
        Interface,

        /// <summary>
        /// The runtime type is an enum.
        /// </summary>
        Enum,
    }
}