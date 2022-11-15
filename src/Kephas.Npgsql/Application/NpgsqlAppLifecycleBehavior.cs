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
    private readonly ILogManager logManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="NpgsqlAppLifecycleBehavior"/> class.
    /// </summary>
    /// <param name="logManager">The log manager.</param>
    public NpgsqlAppLifecycleBehavior(ILogManager logManager)
    {
        this.logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
    }

    /// <summary>
    /// Interceptor called before the application starts its asynchronous initialization.
    /// </summary>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    public Task<IOperationResult> BeforeAppInitializeAsync(CancellationToken cancellationToken = default)
    {
        NpgsqlLogManager.Provider = new NpgsqlLoggingProviderAdapter(this.logManager);

        return Task.FromResult((IOperationResult)true.ToOperationResult());
    }
}
