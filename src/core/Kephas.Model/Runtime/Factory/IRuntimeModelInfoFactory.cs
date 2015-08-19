// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeModelInfoFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for runtime model information providers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Runtime.Factory
{
    using Kephas.Model.Elements.Construction;
    using Kephas.Services;

    /// <summary>
    /// Contract for runtime model information factory.
    /// </summary>
    [SharedAppServiceContract]
    public interface IRuntimeModelInfoFactory
    {
        /// <summary>
        /// Tries to get the named element information.
        /// </summary>
        /// <param name="runtimeElement">The runtime element.</param>
        /// <returns>A named element information or <c>null</c>.</returns>
        INamedElementInfo TryGetModelElementInfo(object runtimeElement);
    }
}