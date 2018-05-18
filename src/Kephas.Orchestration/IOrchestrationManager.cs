// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchestrationManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IOrchestrationManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using Kephas.Services;

    /// <summary>
    /// Interface for orchestration manager.
    /// </summary>
    [SharedAppServiceContract]
    public interface IOrchestrationManager : IAsyncInitializable, IAsyncFinalizable
    {
    }
}