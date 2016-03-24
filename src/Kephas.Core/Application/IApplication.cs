// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplication.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Contract for the root object of the application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract for the root object of the application.
    /// </summary>
    [SharedAppServiceContract]
    public interface IApplication
    {
        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        string AppId { get; }

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
}