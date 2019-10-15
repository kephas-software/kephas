// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStarter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process starter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kephas.Diagnostics
{
    public interface IProcessStarter
    {
        ProcessStartInfo ProcessStartInfo { get; }

        Task<ProcessStartResult> StartAsync(Action<Process> config = null, CancellationToken cancellationToken = default);
    }
}