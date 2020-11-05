// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransition.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model
{
    using Kephas.Model;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// Contract for transitions.
    /// </summary>
    public interface ITransition : IModelElement, ITransitionInfo
    {
    }
}