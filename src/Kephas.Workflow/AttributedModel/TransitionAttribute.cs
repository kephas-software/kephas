// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransitionAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the transition attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute for transition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TransitionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the source states.
        /// </summary>
        /// <value>
        /// The source states.
        /// </value>
        public object[] From { get; set; }

        /// <summary>
        /// Gets or sets the target state.
        /// </summary>
        /// <value>
        /// The target state.
        /// </value>
        public object To { get; set; }
    }
}
