// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using Kephas.Operations;
using Kephas.Services;
using Kephas.Threading.Tasks;

/// <summary>
/// Singleton application service specialized in processing templates of one kind.
/// </summary>
/// <remarks>
/// Typically, the template processor aggregates the templating engines delegating the processing
/// to the specific engine based on the template kind.
/// </remarks>
[SingletonAppServiceContract(AllowMultiple = true)]
public interface ITemplatingEngine
{
    /// <summary>
    /// Processes the provided template asynchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    Task<IOperationResult<object?>> ProcessAsync<T>(
        ITemplate template,
        T? model,
        ITemplateProcessingContext processingContext,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes the provided template synchronously returning the processed output.
    /// </summary>
    /// <typeparam name="T">The type of the bound model.</typeparam>
    /// <param name="template">The template to be interpreted/executed.</param>
    /// <param name="model">Optional. The template model.</param>
    /// <param name="processingContext">The processing context.</param>
    /// <returns>
    /// A promise of the execution result.
    /// </returns>
    IOperationResult<object?> Process<T>(
        ITemplate template,
        T? model,
        ITemplateProcessingContext processingContext)
    {
        return this.ProcessAsync(template, model, processingContext).GetResultNonLocking()!;
    }
}