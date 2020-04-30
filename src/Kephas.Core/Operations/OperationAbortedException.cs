﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationAbortedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;

    /// <summary>
    /// Exception for signalling operation aborted errors.
    /// </summary>
    public class OperationAbortedException : OperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAbortedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public OperationAbortedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationAbortedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public OperationAbortedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}