// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRazorPageCompiler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using Kephas.Operations;
using Kephas.Services;

/// <summary>
/// Service providing compilation for Razor pages.
/// </summary>
[AppServiceContract]
public interface IRazorPageCompiler
{
    /// <summary>
    /// Compiles the template asynchronously.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="template">The template.</param>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The asynchronous result yielding the operation result.</returns>
    Task<IOperationResult<ICompiledRazorPage>> CompileTemplateAsync<TModel>(
        ITemplate template,
        ITemplateProcessingContext context,
        CancellationToken cancellationToken = default);
}