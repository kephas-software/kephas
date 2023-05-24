// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BootstrapException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bootstrap exception class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;

    using Kephas.ExceptionHandling;
    using Kephas.Services.Builder;

    /// <summary>
    /// Exception for signalling bootstrap errors.
    /// </summary>
    public class BootstrapException : ApplicationException, ISeverityQualifiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        public BootstrapException(string message, IAppServiceCollectionBuilder servicesBuilder)
            : base(message)
        {
            this.ServicesBuilder = servicesBuilder;
            this.Severity = SeverityLevel.Fatal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="servicesBuilder">The services builder.</param>
        /// <param name="inner">The inner exception.</param>
        public BootstrapException(string message, IAppServiceCollectionBuilder servicesBuilder, Exception inner)
            : base(message, inner)
        {
            this.ServicesBuilder = servicesBuilder;
            this.Severity = SeverityLevel.Fatal;
        }

        /// <summary>
        /// Gets or sets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        public SeverityLevel Severity { get; set; }

        /// <summary>
        /// Gets or sets a context for the application.
        /// </summary>
        /// <value>
        /// The application context.
        /// </value>
        public IAppContext? AppContext { get; set; }

        /// <summary>
        /// Gets the services builder.
        /// </summary>
        /// <value>
        /// The services builder.
        /// </value>
        public IAppServiceCollectionBuilder ServicesBuilder { get; }
    }
}