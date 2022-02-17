// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateProcessingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating
{
    using System;

    using Kephas.Injection;
    using Kephas.Operations;
    using Kephas.Services;

    /// <summary>
    /// A context for processing templates.
    /// </summary>
    public class TemplateProcessingContext : Context, ITemplateProcessingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessingContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public TemplateProcessingContext(IInjector injector)
            : base(injector)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProcessingContext"/> class.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        public TemplateProcessingContext(IContext executionContext)
            : base(executionContext ?? throw new ArgumentNullException(nameof(executionContext)))
        {
        }

        /// <summary>
        /// Gets or sets the template to process.
        /// </summary>
        /// <value>
        /// The template to process.
        /// </value>
        public ITemplate? Template { get; set; }

        /// <summary>
        /// Gets or sets the bound model.
        /// </summary>
        /// <value>
        /// The bound model.
        /// </value>
        public object? Model { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public IOperationResult<object?>? Result { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception? Exception { get; set; }
    }
}