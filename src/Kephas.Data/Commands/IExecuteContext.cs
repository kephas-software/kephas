// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExecuteContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IExecuteContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    /// <summary>
    /// Interface for execute context.
    /// </summary>
    public interface IExecuteContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        string CommandText { get; }
    }
}