// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataOperationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Services;

    /// <summary>
    /// Contract for data operations contexts.
    /// </summary>
    public interface IDataOperationContext : IContext
    {
        /// <summary>
        /// Gets the dataContext.
        /// </summary>
        /// <value>
        /// The dataContext.
        /// </value>
        IDataContext DataContext { get; }
    }
}