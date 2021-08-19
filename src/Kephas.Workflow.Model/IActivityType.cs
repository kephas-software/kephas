// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivityType.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivityType interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Model
{
    using Kephas.Model;
    using Kephas.Workflow.Reflection;

    /// <summary>
    /// An activity type holds metadata about the activities involved in the workflow processing.
    /// </summary>
    public interface IActivityType : IClassifier, IActivityInfo
    {
    }
}