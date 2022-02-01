// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Services;
using Kephas.Threading.Tasks;

/// <summary>
/// Singleton application service for processing templates.
/// </summary>
[SingletonAppServiceContract]
public interface ITemplateProcessor
{
    /// <summary>
    /// Processes the provided template asynchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    Task<object?> ProcessAsync<T>(
        ITemplate template,
        T? model = default,
        Action<ITemplateProcessingContext>? optionsConfig = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes the provided template synchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="optionsConfig">Optional. The options configuration.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    object? Process<T>(
        ITemplate template,
        T? model = default,
        Action<ITemplateProcessingContext>? optionsConfig = null)
    {
        return this.ProcessAsync(template, model, optionsConfig).GetResultNonLocking();
    }
}