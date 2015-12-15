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
    using Kephas.Dynamic;

    /// <summary>
    /// Contract providing type information.
    /// </summary>
    public interface ITypeInfo : IExpando
    {
        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        string Name { get; }
    }
}