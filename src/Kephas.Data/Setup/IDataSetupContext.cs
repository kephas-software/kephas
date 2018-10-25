// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSetupContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSetupContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup
{
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for data setup context.
    /// </summary>
    public interface IDataSetupContext : IContext
    {
        /// <summary>
        /// Gets the data targets.
        /// </summary>
        /// <value>
        /// The data targets.
        /// </value>
        IEnumerable<string> Targets { get; }
    }
}