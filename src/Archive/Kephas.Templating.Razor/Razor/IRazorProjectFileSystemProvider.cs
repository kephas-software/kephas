// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRazorProjectFileSystemProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// Provider of a <see cref="GetRazorProjectFileSystemAsync"/> out of a template.
/// </summary>
[AppServiceContract]
public interface IRazorProjectFileSystemProvider
{
    /// <summary>
    /// Gets a razor project file system for the provided template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">Optional. the cancellation token.</param>
    /// <returns>A tuple containing the project file system and the base path.</returns>
    Task<(RazorProjectFileSystem project, string? basePath)> GetRazorProjectFileSystemAsync(
        ITemplate template,
        ITemplateProcessingContext context,
        CancellationToken cancellationToken = default);
}