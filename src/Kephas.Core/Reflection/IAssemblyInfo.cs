// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAssemblyInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for assembly information.
    /// </summary>
    public interface IAssemblyInfo : IElementInfo
    {
        /// <summary>
        /// Gets the types declared in this assembly.
        /// </summary>
        /// <value>
        /// The declared types.
        /// </value>
        IEnumerable<ITypeInfo> Types { get; }
    }
}