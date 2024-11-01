// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IApplicationPartsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Kephas.Web.Mvc;

/// <summary>
/// Contract for a provider of application parts.
/// </summary>
public interface IApplicationPartsProvider
{
    /// <summary>
    /// Gets the application parts.
    /// </summary>
    /// <returns>An enumeration of application parts.</returns>
    IEnumerable<ApplicationPart> GetApplicationParts();
}