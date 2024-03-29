﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data;

using Kephas.Services;

/// <summary>
/// Factory service for data contexts.
/// </summary>
/// <remarks>
/// Data contexts are dependent on initialization data,
/// which typically contains at least data store connection information.
/// Due to the fact that multiple physical data stores may be served
/// by the same data context implementation and that by design, at one time,
/// a data context instance may be connected to a single physical data store,
/// there must be a factory service that creates a data context for a given connection.
/// This is the data context factory application service.
/// </remarks>
[SingletonAppServiceContract]
public interface IDataContextFactory
{
    /// <summary>
    /// Creates a data context for the provided data store name and initializes it.
    /// </summary>
    /// <param name="dataStoreName">Name of the data store. This identifies typically
    ///                             an entry in the configuration where connection information
    ///                             and other initialization data is provided.</param>
    /// <param name="initializationContext">Optional. An initialization context.</param>
    /// <returns>
    /// The newly created data context.
    /// </returns>
    IDataContext CreateDataContext(string dataStoreName, IContext? initializationContext = null);
}