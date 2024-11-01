// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationPartsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Runtime;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Kephas.Web.Mvc;

/// <summary>
/// The default implementation of the <see cref="IApplicationPartsProvider"/> service.
/// </summary>
public class ApplicationPartsProvider(IAppRuntime appRuntime) : IApplicationPartsProvider
{
    /// <summary>
    /// Gets the application parts.
    /// </summary>
    /// <returns>An enumeration of application parts.</returns>
    public IEnumerable<ApplicationPart> GetApplicationParts() =>
        appRuntime.GetAppAssemblies().Select(a => new AssemblyPart(a));
}