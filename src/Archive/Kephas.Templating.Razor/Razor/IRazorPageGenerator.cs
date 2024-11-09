// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRazorPageGenerator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Operations;
using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// Result of the razor page generation.
/// </summary>
public record RazorPageGeneratorResult(string FilePath, string GeneratedCode);

/// <summary>
/// Service for generating the C# code for the razor page.
/// </summary>
[AppServiceContract]
public interface IRazorPageGenerator
{
    /// <summary>
    /// Generates the razor page.
    /// </summary>
    /// <param name="projectEngine">The project engine.</param>
    /// <param name="projectItem">The project item.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <returns>An operation result yielding the generation result.</returns>
    IOperationResult<RazorPageGeneratorResult> GenerateRazorPage(
        RazorProjectEngine projectEngine,
        RazorProjectItem projectItem,
        ITemplateProcessingContext processingContext);
}