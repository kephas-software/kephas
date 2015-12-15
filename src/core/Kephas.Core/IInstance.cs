// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInstance.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for instances of classifiers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas
{
    using Kephas.Reflection;

    /// <summary>
    /// Contract for instances of classifiers.
    /// </summary>
    public interface IInstance
    {
        /// <summary>
        /// Gets the type information for this instance.
        /// </summary>
        /// <returns>
        /// The type information.
        /// </returns>
        ITypeInfo GetTypeInfo();
    }
}