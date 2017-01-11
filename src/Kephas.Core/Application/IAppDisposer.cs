// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppDisposer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IAppDisposer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Service contract for an application disposer.
    /// </summary>
    [SharedAppServiceContract]
    [ContractClass(typeof(AppDisposerContractClass))]
    public interface IAppDisposer
    {
        /// <summary>
        /// Disposes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task DisposeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// An application bootstrapper contract class.
    /// </summary>
    [ContractClassFor(typeof(IAppDisposer))]
    internal abstract class AppDisposerContractClass : IAppDisposer
    {
        /// <summary>
        /// Disposes the application asynchronously.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task DisposeAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(appContext != null);
            Contract.Ensures(Contract.Result<Task>() != null);
            return Contract.Result<Task>();
        }
    }
}