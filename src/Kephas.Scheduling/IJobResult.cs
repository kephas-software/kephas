// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJobResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJobResult interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using Kephas.Operations;

    /// <summary>
    /// Interface for job result.
    /// </summary>
    public interface IJobResult : IOperationResult
    {
        /// <summary>
        /// Gets the job or its ID.
        /// </summary>
        /// <value>
        /// The job or its ID.
        /// </value>
        object Job { get; }
    }
}
