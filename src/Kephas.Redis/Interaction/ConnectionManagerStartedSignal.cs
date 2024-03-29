﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionManagerStartedSignal.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the redis client started signal class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Redis.Interaction;

using Kephas.ExceptionHandling;
using Kephas.Interaction;

/// <summary>
/// The Redis connection manager started signal. Issued after the manager completed initialization.
/// </summary>
public class ConnectionManagerStartedSignal : SignalBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionManagerStartedSignal"/> class.
    /// </summary>
    /// <param name="message">Optional. The message.</param>
    /// <param name="severity">Optional. The severity.</param>
    public ConnectionManagerStartedSignal(string? message = null, SeverityLevel severity = SeverityLevel.Info)
        : base(message ?? "The Redis connection manager is started.", severity)
    {
    }
}