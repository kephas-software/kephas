// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppBoostrapperExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for an application boostrapper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for an application boostrapper.
    /// </summary>
    public static class AppBoostrapperExtensions
    {
        /// <summary>
        /// Starts the application asynchronously providing a default application context.
        /// </summary>
        /// <param name="appBootstrapper">  The application boostrapper.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public static Task StartAsync(
            this IAppBootstrapper appBootstrapper,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Contract.Requires(appBootstrapper != null);

            return appBootstrapper.StartAsync(new AppContext(), cancellationToken);
        }
    }
}