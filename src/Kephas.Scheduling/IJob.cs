// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJob.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IJob interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for job.
    /// </summary>
    public interface IJob : IExpando, IInstance, IDisposable
    {
        /// <summary>
        /// Gets the arguments for the execution.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        IExpando Arguments { get; }
    }
}