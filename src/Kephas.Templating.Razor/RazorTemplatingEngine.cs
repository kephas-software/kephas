// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Operations;
using Kephas.Templating.AttributedModel;

namespace Kephas.Templating.Razor;

/// <summary>
/// Templating engine using the cshtml format.
/// </summary>
/// <seealso cref="Kephas.Templating.ITemplatingEngine" />
[TemplateKind("cshtml")]
public class RazorTemplatingEngine : ITemplatingEngine
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
    public Task<IOperationResult<object?>> ProcessAsync<T>(ITemplate template, T? model, ITemplateProcessingContext processingContext, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
