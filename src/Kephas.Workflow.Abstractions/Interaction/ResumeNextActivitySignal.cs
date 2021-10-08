// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResumeNextActivitySignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the resume next activity signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Interaction
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal indicating that the current activity, which terminated with an exception,
    /// should be aborted and the execution should continue with the next activity in the pipeline.
    /// </summary>
    public class ResumeNextActivitySignal : Exception, ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeNextActivitySignal"/> class.
        /// </summary>
        public ResumeNextActivitySignal()
            : this($"Signal {nameof(ResumeNextActivitySignal)}.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeNextActivitySignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ResumeNextActivitySignal(string message)
            : base(message)
        {
            this.Severity = SeverityLevel.Info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResumeNextActivitySignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public ResumeNextActivitySignal(string message, Exception inner)
            : base(message, inner)
        {
            this.Severity = SeverityLevel.Info;
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }
    }
}