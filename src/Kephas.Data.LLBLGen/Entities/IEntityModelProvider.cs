// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityModelProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the illbl generate model provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Entities
{
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Shared application service contract for providing the LLBLGen entity model.
    /// </summary>
    [SharedAppServiceContract]
    public interface IEntityModelProvider
    {
        /// <summary>
        /// Gets the <see cref="ITypeInfo"/>s containing the entity model.
        /// </summary>
        /// <returns>
        /// An enumeration of <see cref="ITypeInfo"/>s.
        /// </returns>
        IEnumerable<ITypeInfo> GetModelTypeInfos();
    }
}