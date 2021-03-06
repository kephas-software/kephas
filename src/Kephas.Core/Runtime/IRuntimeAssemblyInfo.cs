﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeAssemblyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeAssemblyInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for runtime assembly information.
    /// </summary>
    public interface IRuntimeAssemblyInfo : IRuntimeElementInfo, IAssemblyInfo, IEquatable<IRuntimeAssemblyInfo>
    {
        /// <summary>
        /// Gets the underlying assembly information.
        /// </summary>
        /// <returns>
        /// The underlying assembly information.
        /// </returns>
        Assembly GetUnderlyingAssemblyInfo();
    }
}