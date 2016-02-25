// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract providing type information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Contract providing type information.
    /// </summary>
    public interface ITypeInfo : IElementInfo
    {
        /// <summary>
        /// Gets the namespace of the type.
        /// </summary>
        /// <value>
        /// The namespace of the type.
        /// </value>
        string Namespace { get; }
    }
}