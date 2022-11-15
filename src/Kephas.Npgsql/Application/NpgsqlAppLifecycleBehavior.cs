// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NpgsqlAppLifecycleBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the Npgsql feature manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Npgsql.Application;

using System.Threading;
using System.Threading.Tasks;

using global::Npgsql.Logging;
using Kephas.Application;
using Kephas.Logging;
using Kephas.Npgsql.Logging;
using Kephas.Operations;

/// <summary>
/// The Npgsql application lifecycle behavior.
/// </summary>
public class NpgsqlAppLifecycleBehavior : IAppLifecycleBehavior
{
    /// <summary>
    /// Interceptor called before the application starts its asynchronous initialization.
    /// </summary>
    /// <param name="appContext">Context for the application.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    public Task<IOperationResult> BeforeAppInitializeAsync(
        IAppContext appContext,
        CancellationToken cancellationToken = default)
    {
        NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(appContext.ServiceProvider.Resolve<ILogManager>());

        return Task.FromResult((IOperationResult)true.ToOperationResult());
    }
}
