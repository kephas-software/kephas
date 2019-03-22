// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IConfigurationStore interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Configuration
{
    using Kephas.Dynamic;

    /// <summary>
    /// Contract interface for the configuration store.
    /// </summary>
    /// <remarks>
    /// The configuration store is used to store configuration values based on keys.
    /// </remarks>
    public interface IConfigurationStore : IIndexable
    {
    }
}