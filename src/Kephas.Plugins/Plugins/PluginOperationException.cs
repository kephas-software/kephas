// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryOperationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the retry operation exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// Exception for signalling plugin operation errors.
    /// </summary>
    public class PluginOperationException : Exception, ISeverityQualifiedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOperationException"/> class.
        /// </summary>
        /// <param name="result">Optional. The operation result.</param>
        /// <param name="severity">Optional. The severity.</param>
        public PluginOperationException(IOperationResult result = null, SeverityLevel severity = SeverityLevel.Error)
        {
            this.Severity = severity;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="result">Optional. The operation result.</param>
        /// <param name="severity">Optional. The severity.</param>
        public PluginOperationException(string message, IOperationResult result = null, SeverityLevel severity = SeverityLevel.Error)
            : base(message)
        {
            this.Severity = severity;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginOperationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="result">Optional. The operation result.</param>
        /// <param name="severity">Optional. The severity.</param>
        public PluginOperationException(string message, Exception inner, IOperationResult result = null, SeverityLevel severity = SeverityLevel.Error)
            : base(message, inner)
        {
            this.Severity = severity;
            this.Result = result;
        }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; }

        /// <summary>
        /// Gets the operation result.
        /// </summary>
        /// <value>
        /// The operation result.
        /// </value>
        public IOperationResult Result { get; }
    }
}
