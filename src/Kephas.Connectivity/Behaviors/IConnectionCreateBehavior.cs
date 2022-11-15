// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConnectionCreateBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Connectivity.Behaviors;

using Kephas.Services;

/// <summary>
/// Service contract for behaviors during connection creation.
/// </summary>
[AppServiceContract(AllowMultiple = true)]
public interface IConnectionCreateBehavior
{
    /// <summary>
    /// Interception called before creating the connection.
    /// </summary>
    /// <param name="context">The connection context.</param>
    void BeforeCreate(IConnectionContext context);

    /// <summary>
    /// Interception called after creating the connection.
    /// </summary>
    /// <param name="context">The connection context.</param>
    /// <remarks>
    /// The context will contain the connection or the exception during creation.
    /// The interceptor may change the connection or even replace it with another one.
    /// </remarks>
    void AfterCreate(IConnectionContext context);
}