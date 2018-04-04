// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeAssemblyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeAssemblyInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for runtime assembly information.
    /// </summary>
    public interface IRuntimeAssemblyInfo : IRuntimeElementInfo, IAssemblyInfo
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