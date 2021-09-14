// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeAppInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeAppInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration
{
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for runtime application information.
    /// </summary>
    public interface IRuntimeAppInfo : IDynamic
    {
    /// <summary>
    /// Gets the identifier of the application.
    /// </summary>
    /// <value>
    /// The identifier of the application.
    /// </value>
    string AppId { get; }

    /// <summary>
    /// Gets the identifier of the application instance.
    /// </summary>
    /// <value>
    /// The identifier of the application instance.
    /// </value>
    string AppInstanceId { get; }

    /// <summary>
    /// Gets a value indicating whether the application is the root.
    /// </summary>
    bool IsRoot { get; }

    /// <summary>
    /// Gets the aupported app features.
    /// </summary>
    /// <value>
    /// The supported app features.
    /// </value>
    string[] Features { get; }

    /// <summary>
    /// Gets the identifier of the process running the application.
    /// </summary>
    /// <value>
    /// The identifier of the process.
    /// </value>
    int ProcessId { get; }

    /// <summary>
    /// Gets the name of the host where the application process runs.
    /// </summary>
    /// <value>
    /// The name of the host.
    /// </value>
    string HostName { get; }

    /// <summary>
    /// Gets the address of the host where the application process runs.
    /// </summary>
    /// <value>
    /// The host address.
    /// </value>
    string HostAddress { get; }

    /// <summary>
    /// Gets or sets the application properties.
    /// </summary>
    /// <value>
    /// The application properties.
    /// </value>
    IDictionary<string, object> Properties { get; set; }
    }
}