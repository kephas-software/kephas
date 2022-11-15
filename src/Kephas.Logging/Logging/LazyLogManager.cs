// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LazyLogManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging;

using System.Text;
using Kephas.Diagnostics.Logging;

/// <summary>
/// A log manager receiving a 
/// </summary>
public class LazyLogManager : ILogManager, IAdapter<ILogManager>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LazyLogManager"/> class.
    /// </summary>
    public LazyLogManager()
    {
        this.Of = new DebugLogManager(new StringBuilder());
    }

    /// <summary>
    /// Gets or sets the global minimum level.
    /// </summary>
    public LogLevel MinimumLevel
    {
        get => this.Of.MinimumLevel;
        set => this.Of.MinimumLevel = value;
    }

    public ILogger GetLogger(string loggerName) => this.Of.GetLogger(loggerName);

    public ILogManager Of { get; }
}