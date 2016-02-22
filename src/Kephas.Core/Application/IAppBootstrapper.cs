// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppBootstrapper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Service contract for an application bootstrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Service contract for an application bootstrapper.
    /// </summary>
    [SharedAppServiceContract]
    [ContractClass(typeof(AppBootstrapperContractClass))]
    public interface IAppBootstrapper
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// An application bootstrapper contract class.
    /// </summary>
    [ContractClassFor(typeof(IAppBootstrapper))]
    internal abstract class AppBootstrapperContractClass : IAppBootstrapper
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <param name="appContext">       Context for the application.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task StartAsync(IAppContext appContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(appContext != null);
            Contract.Ensures(Contract.Result<Task>() != null);
            return Contract.Result<Task>();
        }
    }
}