// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatingException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Operations;

    /// <summary>
    /// Exception for signalling template processing errors.
    /// </summary>
    public class TemplatingException : Exception, ISeverityQualifiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatingException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="template">Optional. The template.</param>
        /// <param name="processingResult">Optional. The processing result.</param>
        public TemplatingException(string message, ITemplate? template = null, IOperationResult? processingResult = null)
            : base(message)
        {
            this.Severity = SeverityLevel.Error;
            this.Template = template;
            this.ProcessingResult = processingResult;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatingException"/>
        ///  class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        /// <param name="template">Optional. The template.</param>
        /// <param name="processingResult">Optional. The processing result.</param>
        public TemplatingException(string message, Exception inner, ITemplate? template = null, IOperationResult? processingResult = null)
            : base(message, inner)
        {
            this.Severity = SeverityLevel.Error;
            this.Template = template;
            this.ProcessingResult = processingResult;
        }

        /// <summary>
        /// Gets the template.
        /// </summary>
        public ITemplate? Template { get; }

        /// <summary>
        /// Gets the processing result.
        /// </summary>
        public IOperationResult? ProcessingResult { get; }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; set; }
    }
}