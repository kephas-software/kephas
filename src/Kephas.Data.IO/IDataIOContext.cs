// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataIOContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataIOContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO
{
    using System;
    using System.Threading;

    using Kephas.Services;

    /// <summary>
    /// Interface for data i/o context.
    /// </summary>
    public interface IDataIOContext : IContext
    {
        /// <summary>
        /// Gets or sets the cancellation token source.
        /// </summary>
        /// <value>
        /// The cancellation token source.
        /// </value>
        CancellationTokenSource CancellationTokenSource { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IDataIOContext"/>.
    /// </summary>
    public static class DataIOContextExtensions
    {
        /// <summary>
        /// Checks whether the cancellation was requested, and if this is the case an <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void CheckCancellationRequested(this IDataIOContext context)
        {
            if (context?.CancellationTokenSource == null)
            {
                return;
            }

            if (context.CancellationTokenSource.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}