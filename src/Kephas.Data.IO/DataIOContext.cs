// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data i/o context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System.Threading;

    using Kephas.Services;

    /// <summary>
    /// A data i/o context.
    /// </summary>
    public class DataIOContext : Context, IDataIOContext
    {
        /// <summary>
        /// Gets or sets the cancellation token source.
        /// </summary>
        /// <value>
        /// The cancellation token source.
        /// </value>
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }
}