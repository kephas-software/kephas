// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerAppContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting.Application
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;

    /// <summary>
    /// An application context extensions.
    /// </summary>
    internal static class WorkerAppContextExtensions
    {
        /// <summary>
        /// Initializes the application manager asynchronously.
        /// </summary>
        /// <param name="appContext">The appContext to act on.</param>
        /// <returns>
        /// A function delegate that yields a Task&lt;IAppContext&gt;.
        /// </returns>
        public static Func<CancellationToken, Task<IAppContext>> InitializeAppManagerAsync(this IAppContext appContext)
        {
            return appContext[nameof(InitializeAppManagerAsync)] as Func<CancellationToken, Task<IAppContext>>;
        }

        /// <summary>
        /// Initializes the application manager asynchronously.
        /// </summary>
        /// <param name="appContext">The appContext to act on.</param>
        /// <param name="initializeAsync">The initialize asynchronous.</param>
        /// <returns>
        /// A function delegate that yields a Task&lt;IAppContext&gt;.
        /// </returns>
        public static IAppContext InitializeAppManagerAsync(this IAppContext appContext, Func<CancellationToken, Task<IAppContext>> initializeAsync)
        {
            appContext[nameof(InitializeAppManagerAsync)] = initializeAsync;
            return appContext;
        }
    }
}
