// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchestrationException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Signals that an orchestration exception occured.
    /// </summary>
    [Serializable]
    public class OrchestrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OrchestrationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchestrationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public OrchestrationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}