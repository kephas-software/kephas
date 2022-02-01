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
/// Singleton application service contract responsible for adding behavior to script execution for a specified language.
/// </summary>
[SingletonAppServiceContract(AllowMultiple = true)]
public interface ITemplateProcessingBehavior
{
    /// <summary>
    /// Interception called before invoking the language service to execute the script.
    /// </summary>
    /// <param name="processingContext">Information describing the execution.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// A task.
    /// </returns>
    Task BeforeProcessAsync(ITemplateProcessingContext processingContext, CancellationToken token);

    /// <summary>
    /// Interception called after invoking the language service to execute the script.
    /// </summary>
    /// <remarks>
    /// The execution data contains the execution result. 
    /// The interceptor may change the result or even replace it with another one.
    /// </remarks>
    /// <param name="processingContext">Information describing the execution.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// A task.
    /// </returns>
    Task AfterProcessAsync(ITemplateProcessingContext processingContext, CancellationToken token);
}