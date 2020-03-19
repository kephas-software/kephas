// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransitionInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITransitionInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Workflow.Reflection
{
    using System.Collections.Generic;

    using Kephas.Reflection;

    /// <summary>
    /// Contract interface for transition metadata.
    /// </summary>
    public interface ITransitionInfo : IOperationInfo
    {
        /// <summary>
        /// Gets the states from which the transitions starts.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        IEnumerable<object> From { get; }

        /// <summary>
        /// Gets the state to which the state machine is transitioned.
        /// </summary>
        /// <value>
        /// An enumeration of states.
        /// </value>
        object To { get; }
    }
}
