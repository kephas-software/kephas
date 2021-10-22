// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAssemblyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic assembly information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Dynamic
{
    using System.Collections.Generic;

    /// <summary>
    /// Information about the dynamic assembly.
    /// </summary>
    public class DynamicAssemblyInfo : DynamicTypeRegistry, IAssemblyInfo
    {
        /// <summary>
        /// Gets the types declared in this assembly.
        /// </summary>
        /// <value>
        /// The declared types.
        /// </value>
        IEnumerable<ITypeInfo> IAssemblyInfo.Types => this.Types;
    }
}