﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHubService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Marker interface for hubs.
    /// </summary>
    [AppServiceContract(AllowMultiple = true)]
    public interface IHubService : IDisposable
    {
    }
}