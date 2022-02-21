// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleRazorProjectFileSystemProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Services;
using Kephas.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// The simple implementation of the <see cref="IRazorProjectFileSystemProvider"/>
/// handling a single template file without any additional template references.
/// </summary>
[OverridePriority(Priority.Low)]
public class SimpleRazorProjectFileSystemProvider : IRazorProjectFileSystemProvider
{
    /// <summary>
    /// Gets a razor project file system for the provided template.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">Optional. the cancellation token.</param>
    /// <returns>A tuple containing the project file system and the base path.</returns>
    public async Task<(RazorProjectFileSystem project, string? basePath)> GetRazorProjectFileSystemAsync(
        ITemplate template,
        ITemplateProcessingContext context,
        CancellationToken cancellationToken = default)
    {
        var projectDirectory = await this.CreateProjectDirectoryAsync(template, cancellationToken)
            .PreserveThreadContext();
        return (RazorProjectFileSystem.Create(projectDirectory), projectDirectory);
    }

    private static string GetProjectTempFolder()
    {
        return $"tpl-{DateTime.Now:s}-{Guid.NewGuid():N}"
            .Replace(":", string.Empty)
            .Replace("-", string.Empty);
    }

    private async Task<string> CreateProjectDirectoryAsync(ITemplate template, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tempFolder = Path.GetTempPath();
        var projectDirectory = Path.Combine(tempFolder, GetProjectTempFolder());
        Directory.CreateDirectory(projectDirectory);

        await File.WriteAllTextAsync(
            Path.Combine(projectDirectory, "template.cshtml"),
            await template.GetContentAsync(cancellationToken).PreserveThreadContext(),
            cancellationToken).PreserveThreadContext();

        return projectDirectory;
    }
}