// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISynchronizedHostedService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Interface for synchronized hosted services.
/// </summary>
[SingletonService(MetadataType = typeof(SynchronizedHostedServiceMetadata))]
public interface ISynchronizedHostedService : IHostedService
{
    internal Task OnStartAsync(CancellationToken cancellationToken);

    internal Task OnStopAsync(CancellationToken cancellationToken);
}