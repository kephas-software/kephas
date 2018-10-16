// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scoping.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scope kind class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Security.Authorization
{
    using System;

    /// <summary>
    /// Values that represent permission scoping.
    /// </summary>
    [Flags]
    public enum Scoping
    {
        /// <summary>
        /// Scoping is not supported by this permission type.
        /// </summary>
        /// <remarks>
        /// This value is provided as default, although it is more or less invalid.
        /// A permission with scoping <see cref="None"/> is not usable.
        /// </remarks>
        None = 0x00,
        
        /// <summary>
        /// No scoping required for this permission type, it will be granted and verified at global level.
        /// </summary>
        Global = 0x0001,

        /// <summary>
        /// The scoping is granted and verified at type level.
        /// </summary>
        Type = 0x0010,

        /// <summary>
        /// The scoping is granted and verified at instance level.
        /// </summary>
        Instance = 0x0100,
    }
}