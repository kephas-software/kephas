// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenApiSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.OpenApi;

/// <summary>
/// The configuration section used to configure NSwag.
/// </summary>
public sealed class OpenApiSettings
{
    /// <summary>
    /// Gets or sets the title.
    /// </summary>
    /// <value>The title.</value>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Enables/disables Swagger endpoint.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the supported versions.
    /// </summary>
    public int[] Versions { get; set; } = [1];
}