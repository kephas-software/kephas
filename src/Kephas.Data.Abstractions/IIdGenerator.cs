// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIdGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IIdGenerator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Data;

/// <summary>
/// Contract for a singleton application service generating ID values.
/// </summary>
[SingletonAppServiceContract]
public interface IIdGenerator
{
    /// <summary>
    /// Generates a unique identifier.
    /// </summary>
    /// <returns>
    /// The unique identifier.
    /// </returns>
    long GenerateId();
}
