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
        /// Initializes a new instance of the <see cref="TransitionAttribute"/> class.
        /// </summary>
        /// <param name="from">The source state.</param>
        /// <param name="to">The target state.</param>
        public TransitionAttribute(object from, object to)
        {
            from = from ?? throw new ArgumentNullException(nameof(from));

            this.From = new[] { from };
            this.To = to ?? throw new ArgumentNullException(nameof(to));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionAttribute"/> class.
        /// </summary>
        /// <param name="from">The source state.</param>
        /// <param name="to">The target state.</param>
        public TransitionAttribute(object[] from, object to)
        {
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.To = to ?? throw new ArgumentNullException(nameof(to));
        }

        /// <summary>
        /// Gets the source states.
        /// </summary>
        /// <value>
        /// The source states.
        /// </value>
        public object[] From { get; }

        /// <summary>
        /// Gets the target state.
        /// </summary>
        /// <value>
        /// The target state.
        /// </value>
        public object To { get; }
    }
}
