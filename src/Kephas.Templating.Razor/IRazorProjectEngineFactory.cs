// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRazorProjectEngineFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Services;
using Microsoft.AspNetCore.Razor.Language;

/// <summary>
/// Service for creating a <see cref="RazorProjectEngine"/>.
/// </summary>
[SingletonAppServiceContract]
public interface IRazorProjectEngineFactory
{
    /// <summary>
    /// Creates the project engine for the provided file system and with the given context.
    /// </summary>
    /// <param name="fileSystem">The file system.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <returns>The Razor project engine.</returns>
    RazorProjectEngine CreateProjectEngine(
        RazorProjectFileSystem fileSystem,
        ITemplateProcessingContext processingContext);
}