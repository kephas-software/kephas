// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityInfoAware.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityInfoAware interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Annotates the entities which are aware about there entity information.
    /// </summary>
    public interface IEntityInfoAware
    {
        /// <summary>
        /// Gets the entity information.
        /// </summary>
        /// <returns>
        /// The entity information.
        /// </returns>
        IEntityInfo GetEntityInfo();

        /// <summary>
        /// Sets the entity information.
        /// </summary>
        /// <param name="entityInfo">Information describing the entity.</param>
        void SetEntityInfo(IEntityInfo entityInfo);
    }
}