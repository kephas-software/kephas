// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyAppManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Services;

    [OverridePriority(Priority.Highest)]
    public class DummyAppManager : IAppManager
    {
        public Task InitializeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task FinalizeAppAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}