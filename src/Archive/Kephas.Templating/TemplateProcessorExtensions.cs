// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplateProcessorExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System.Diagnostics.CodeAnalysis;
using Kephas.Templating.Interpolation;

/// <summary>
/// Extension methods for <see cref="ITemplateProcessor"/>.
/// </summary>
public static class TemplateProcessorExtensions
{
    /// <summary>
    /// Interpolates the specified template.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="processor">The processor.</param>
    /// <param name="template">The template.</param>
    /// <param name="model">The model.</param>
    /// <returns>The interpolated string.</returns>
    public static string? Process<T>([DisallowNull] this ITemplateProcessor processor, string template, T model)
    {
        processor = processor ?? throw new ArgumentNullException(nameof(processor));
        var result = processor.Process(new StringTemplate(template, InterpolationTemplatingEngine.Interpolation), model);
        return result as string;
    }

    /// <summary>
    /// Processes the provided template file. The file extension will discriminate the processing engine.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="processor">The processor.</param>
    /// <param name="templatePath">The template path.</param>
    /// <param name="model">The model.</param>
    /// <returns>The interpolated string.</returns>
    public static string? ProcessFile<T>([DisallowNull] this ITemplateProcessor processor, string templatePath, T model)
    {
        processor = processor ?? throw new ArgumentNullException(nameof(processor));
        var result = processor.Process(new FileTemplate(templatePath), model);
        return result as string;
    }
}