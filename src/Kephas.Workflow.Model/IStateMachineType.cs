// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateMachineType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IStateMachineType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Model
{
    using Kephas.Model;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// A state machine type holds metadata about the state machines involved in the workflow processing.
    /// </summary>
    public interface IStateMachineType : IClassifier, IStateMachineInfo
    {
    }
}
