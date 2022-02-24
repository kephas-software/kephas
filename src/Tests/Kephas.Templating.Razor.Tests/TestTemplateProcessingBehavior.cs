namespace Kephas.Templating.Razor.Tests;

using Kephas.Templating.AttributedModel;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;

[TemplateKind(RazorTemplatingEngine.Cshtml)]
public class TestTemplateProcessingBehavior : ITemplateProcessingBehavior
{
    /// <summary>
    /// Interception invoked before the template is processed.
    /// It adds also the PageDirective to the engine.
    /// </summary>
    /// <param name="processingContext">Information describing the processing.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>
    /// The asynchronous result.
    /// </returns>
    public Task BeforeProcessAsync(ITemplateProcessingContext processingContext, CancellationToken token)
    {
        var existingConfig = processingContext.ConfigureEngine();
        processingContext.ConfigureEngine(engine =>
        {
            existingConfig?.Invoke(engine);
            PageDirective.Register(engine);
        });

        return Task.CompletedTask;
    }
}