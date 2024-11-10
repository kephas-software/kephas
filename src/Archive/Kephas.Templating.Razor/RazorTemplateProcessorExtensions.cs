// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplateProcessorExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System.Diagnostics.CodeAnalysis;
using Kephas.Templating.Interpolation;
using Kephas.Templating.Razor;

/// <summary>
/// Extension methods for <see cref="ITemplateProcessor"/>.
/// </summary>
public static class RazorTemplateProcessorExtensions
{
    /// <summary>
    /// Processes the provided template with Razor.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="processor">The processor.</param>
    /// <param name="template">The template.</param>
    /// <param name="model">The model.</param>
    /// <returns>The interpolated string.</returns>
    public static string? ProcessWithRazor<T>([DisallowNull] this ITemplateProcessor processor, string template, T model)
    {
        processor = processor ?? throw new ArgumentNullException(nameof(processor));
        var result = processor.Process(new StringTemplate(template, RazorTemplatingEngine.Cshtml), model);
        return result as string;
    }
}