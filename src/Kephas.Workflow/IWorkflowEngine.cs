// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowEngine.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWorkflowEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using Kephas.Services;

    /// <summary>
    /// Shared application service for executing activities.
    /// </summary>
    [SharedAppServiceContract]
    public interface IWorkflowEngine
    {
    }
}