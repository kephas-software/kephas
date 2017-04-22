// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMethodInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for method information.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    /// <summary>
    /// Contract for method information.
    /// </summary>
    public interface IMethodInfo : IElementInfo
    {
        /// <summary>
        /// Gets the return type of the method.
        /// </summary>
        /// <value>
        /// The return type of the method.
        /// </value>
        ITypeInfo ReturnType { get; }
    }
}