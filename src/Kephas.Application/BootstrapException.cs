﻿// --------------------------------------------------------------------------------------------------------------------
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

    /// <summary>
    /// Exception for signalling bootstrap errors.
    /// </summary>
    public class BootstrapException : ApplicationException, ISeverityQualifiedNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BootstrapException(string message)
            : base(message)
        {
            this.Severity = SeverityLevel.Fatal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootstrapException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public BootstrapException(string message, Exception inner)
            : base(message, inner)
        {
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
        public IAppContext AppContext { get; set; }

        /// <summary>
        /// Gets or sets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; set; }
    }
}