// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IComposableContainerAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IComposableContainerAdapter interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ServiceStack.Composition
{
    using global::ServiceStack.Configuration;

    using Kephas.Services;

    /// <summary>
    /// Shared container adapter application service contract.
    /// </summary>
    [SharedAppServiceContract]
    public interface IComposableContainerAdapter : IContainerAdapter
    {
    }
}