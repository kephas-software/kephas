// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryActivitySignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the retry activity signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Interaction
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Interaction;

    /// <summary>
    /// Signal used for retrying an activity.
    /// </summary>
    public class RetryActivitySignal : Exception, ISignal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryActivitySignal"/> class.
        /// </summary>
        /// <remarks>
        /// If no activity context is specified, the current executed activity should be retried.
        /// If no maximum retry count is specified, the retries are performed an unlimited number of times.
        /// </remarks>
        /// <param name="activityContext">Optional. The activity context (in the parent hierarchy)
        ///                                from which the retry should continue.</param>
        /// <param name="maxRetryCount">Optional. The maximum number of retries.</param>
        public RetryActivitySignal(IActivityContext activityContext = null, int? maxRetryCount = null)
            : this($"Signal {typeof(RetryActivitySignal).Name}.", activityContext, maxRetryCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryActivitySignal"/> class.
        /// </summary>
        /// <remarks>
        /// If no activity context is specified, the current executed activity should be retried.
        /// If no maximum retry count is specified, the retries are performed an unlimited number of times.
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <param name="activityContext">Optional. The activity context (in the parent hierarchy)
        ///                                from which the retry should continue.</param>
        /// <param name="maxRetryCount">Optional. The maximum number of retries.</param>
        public RetryActivitySignal(string message, IActivityContext activityContext = null, int? maxRetryCount = null)
            : base(message)
        {
            this.ActivityContext = activityContext;
            this.MaxRetryCount = maxRetryCount;
            this.Severity = SeverityLevel.Info;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryActivitySignal"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="activityContext">The execution context (in the parent hierarchy)
        ///                                from which the retry should continue (optional).</param>
        /// <param name="maxRetryCount">The maximum number of retries (optional).</param>
        /// <remarks>
        /// If no execution context is specified, the current executed task should be retried. If no
        /// maximum retry count is specified, the retries are performed an unlimited number of times.
        /// </remarks>
        public RetryActivitySignal(string message, Exception inner, IActivityContext activityContext = null, int? maxRetryCount = null)
            : base(message, inner)
        {
            this.ActivityContext = activityContext;
            this.MaxRetryCount = maxRetryCount;
            this.Severity = SeverityLevel.Info;
        }

        /// <summary>
        /// Gets a context for the activity.
        /// </summary>
        /// <value>
        /// The activity context.
        /// </value>
        public IActivityContext ActivityContext { get; }

        /// <summary>
        /// Gets the number of maximum retries.
        /// </summary>
        /// <value>
        /// The number of maximum retries.
        /// </value>
        public int? MaxRetryCount { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }
    }
}