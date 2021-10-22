// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Logger abstract interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Logging
{
    using Kephas.Services;

    /// <summary>
    /// Defines a service contract for a logger associated to a specific service.
    /// </summary>
    /// <typeparam name="TContract">The type of the service contract.</typeparam>
    [SingletonAppServiceContract(AsOpenGeneric = true)]
    public interface ILogger<TContract> : ILogger
    {
    }
}