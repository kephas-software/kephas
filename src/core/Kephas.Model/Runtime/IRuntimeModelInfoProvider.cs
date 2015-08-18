// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelInfoProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for runtime model information providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime
{
    using Kephas.Model.Elements.Construction;
    using Kephas.Model.Factory;

    /// <summary>
    /// Contract for runtime model information providers.
    /// </summary>
    public interface IRuntimeModelInfoProvider : IModelInfoProvider
    {
        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>A named element information or <c>null</c>.</returns>
        INamedElementInfo TryGetModelElementInfo(object runtimeElement);
    }
}