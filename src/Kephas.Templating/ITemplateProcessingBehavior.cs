// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITemplateProcessingBehavior.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating;

using System.Threading;
using System.Threading.Tasks;

using Kephas.Services;

/// <summary>
/// Contract for application services responsible for adding behavior to template processing for a specified template kind.
/// </summary>
[AppServiceContract(AllowMultiple = true)]
public interface ITemplateProcessingBehavior
{
    /// <summary>
    /// Interception invoked before the template is processed.
    /// </summary>
    /// <param name="processingContext">Information describing the processing.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    Task BeforeProcessAsync(ITemplateProcessingContext processingContext, CancellationToken token) => Task.CompletedTask;

    /// <summary>
    /// Interception invoked after the template is processed.
    /// </summary>
    /// <remarks>
    /// The interceptor may change the result or even replace it with another one.
    /// </remarks>
    /// <param name="processingContext">Information describing the processing.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    Task AfterProcessAsync(ITemplateProcessingContext processingContext, CancellationToken token) => Task.CompletedTask;
}