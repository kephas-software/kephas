// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RazorTemplatingEngine.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor;

using System.Text;
using Kephas.Logging;
using Kephas.Operations;
using Kephas.Services;
using Kephas.Templating.AttributedModel;
using Kephas.Threading.Tasks;

/// <summary>
/// Templating engine using the cshtml format.
/// </summary>
/// <seealso cref="Kephas.Templating.ITemplatingEngine" />
[TemplateKind("cshtml")]
[ProcessingPriority(Priority.Low)]
public class RazorTemplatingEngine : Loggable, ITemplatingEngine
{
    private readonly IRazorPageCompiler pageCompiler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RazorTemplatingEngine" /> class.
    /// </summary>
    /// <param name="pageCompiler">The page compiler.</param>
    /// <param name="logManager">The log manager.</param>
    public RazorTemplatingEngine(
        IRazorPageCompiler pageCompiler,
        ILogManager? logManager = null)
        : base(logManager)
    {
        this.pageCompiler = pageCompiler ?? throw new ArgumentNullException(nameof(pageCompiler));
    }

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
    public async Task<IOperationResult> ProcessAsync<T>(ITemplate template, T? model, ITemplateProcessingContext processingContext, CancellationToken cancellationToken = default)
    {
        var result = new OperationResult();
        var compiledPageResult = await this.pageCompiler
            .CompileTemplateAsync<T>(template, processingContext, cancellationToken).PreserveThreadContext();
        result.MergeMessages(compiledPageResult);
        if (result.HasErrors())
        {
            return result.Fail(new TemplatingException("Errors occurred during page compilation."));
        }

        var compiledPage = compiledPageResult.Value;

        var ownsWriter = processingContext.TextWriter is null;
        if (ownsWriter)
        {
            var sb = new StringBuilder();
            processingContext.TextWriter = new StringWriter(sb);
        }

        await compiledPage.RenderAsync(model, processingContext.TextWriter!, cancellationToken).PreserveThreadContext();

        if (ownsWriter)
        {
            using var writer = processingContext.TextWriter!;
            result.Value(((StringWriter)writer).GetStringBuilder().ToString());
        }

        return result.Complete();
    }
}
