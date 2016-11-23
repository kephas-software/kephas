// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOnInitializeBehavior.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IOnInitializeBehavior interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Behaviors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract for the behavior invoked upon entity intialization.
    /// </summary>
    public interface IOnInitializeBehavior
    {
        /// <summary>
        /// Initializes the entity asynchronously.
        /// </summary>
        /// <param name="obj">The object to be initialized.</param>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task InitializeAsync(object obj, IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}