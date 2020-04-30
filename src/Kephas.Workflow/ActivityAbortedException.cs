// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityAbortedException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    using Kephas.Operations;

    /// <summary>
    /// Exception signalling activity aborted errors.
    /// </summary>
    public class ActivityAbortedException : OperationAbortedException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAbortedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ActivityAbortedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityAbortedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner exception.</param>
        public ActivityAbortedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}