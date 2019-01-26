// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActivity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IActivity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using Kephas.Dynamic;

    /// <summary>
    /// Base contract for activities.
    /// </summary>
    public interface IActivity : IExpando, IInstance
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