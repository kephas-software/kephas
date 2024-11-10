namespace Kephas.Templating.Tests;

using System.Text;
using NUnit.Framework;

public abstract class DefaultTemplateProcessorTestBase : TemplatingTestBase
{
    protected abstract IServiceProvider BuildServiceProvider(params Type[] parts);

    [Test]
    public void DefaultTemplateProcessor_Injection_success()
    {
        var container = this.BuildServiceProvider();
        var templateProcessor = container.Resolve<ITemplateProcessor>();
        Assert.IsInstanceOf<DefaultTemplateProcessor>(templateProcessor);

        var typedTemplateProcessor = (DefaultTemplateProcessor)templateProcessor;
        Assert.IsNotNull(typedTemplateProcessor.Logger);
    }

    [Test]
    public async Task ProcessAsync_Injection_success()
    {
        var container = this.BuildServiceProvider(parts: new[] { typeof(TestTemplatingEngine) });
        var templateProcessor = container.Resolve<ITemplateProcessor>();

        var template = new StringTemplate("dummy", "test", "test-template");
        var result = await templateProcessor.ProcessAsync<object>(template);

        Assert.AreEqual("processed test-template", result);
    }

    [Test]
    public async Task ProcessAsync_Injection_success_with_writer()
    {
        var container = this.BuildServiceProvider(parts: new[] { typeof(TestTemplatingEngine) });
        var templateProcessor = container.Resolve<ITemplateProcessor>();
        using var writer = new StringWriter();

        var template = new StringTemplate("dummy", "test", "test-template");
        var result = await templateProcessor.ProcessAsync<object>(template, optionsConfig: ctx => ctx.TextWriter = writer);

        Assert.AreEqual("processed test-template", writer.GetStringBuilder().ToString());
        Assert.IsNull(result);
    }

    [Test]
    public async Task ProcessAsync_Injection_failure_missing_handler()
    {
        var container = this.BuildServiceProvider();
        var templateProcessor = container.Resolve<ITemplateProcessor>();

        var template = new StringBuilderTemplate(new StringBuilder("dummy"), "test", "test-template");
        Assert.ThrowsAsync<TemplatingException>(() => templateProcessor.ProcessAsync<object>(template));
    }
}