// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEventHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.Queues;

/// <summary>
/// Asynchronous event handler.
/// </summary>
/// <typeparam name="T">The event arguments type.</typeparam>
/// <param name="sender">The sender.</param>
/// <param name="eventArgs">The event arguments.</param>
/// <returns>The asynchronous result.</returns>
public delegate Task AsyncEventHandler<in T>(object sender, T eventArgs)
    where T : EventArgs;