// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelSpaceProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for providing a model space.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Services;

    /// <summary>
    /// Contract for providing a model space.
    /// </summary>
    [SharedAppServiceContract]
    public interface IModelSpaceProvider : IAsyncInitializable
    {
        /// <summary>
        /// Gets the model space.
        /// </summary>
        /// <returns>The model space.</returns>
        IModelSpace GetModelSpace();
    }
}