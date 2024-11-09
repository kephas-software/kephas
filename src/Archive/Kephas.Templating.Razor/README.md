# Razor Templating

## Introduction
This package provides the implementation of templating using Razor syntax and *.cshtml files.
The ```cshtml``` template kind is handled by the ```RazorTemplatingEngine```.

Check the following packages for more information:
* [Kephas.Templating](https://www.nuget.org/packages/Kephas.Templating)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

## Usage

### Example of a template: template.cshtml file

```html
@model TemplateModel
<div>Hi @Process(Model.Name)!</div>
@functions {
    private string Process(string str)
    {
        return $"@{str}@";
    }
}
```

### Example of code using this template

* File based template processing

```C#
record TemplateModel(string Name);

// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<ITemplateProcessor>();

var result = processor.Process(new FileTemplate("C:\\Path\\To\\template.cshtml"), new TemplateModel("Johnny"));
Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);

// this is a simpler alternative for Razor file templates.
var result = processor.ProcessWithFile("C:\\Path\\To\\template.cshtml", new TemplateModel("Johnny"));
Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);
```

* String based template processing

```C#
record TemplateModel(string Name);

// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<ITemplateProcessor>();

var template = @"
@model TemplateModel
<div>Hi @Process(Model.Name)!</div>
@functions {
    private string Process(string str)
    {
        return $""@{str}@"";
    }
}
";
var result = processor.ProcessWithRazor(template, new TemplateModel("Johnny"));
Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);
```

## Fine tuning the template processing

By default, the following directives are supported by the templating engine:
* Namespace
* Functions
* Inherits
* Section
* Model

For further engine builder configuration, this can be achieved in two ways:
* Provide a configuration lambda expression when invoking the processing.
  * This is a very handy solution but it is not scalable.
If there are a lot of templates to process it is error prone to specify the configuration with each processing invocation.
* Use a ```ITemplatingProcessingBehavior``` implementation.
  * This is an advanced solution. Override the ```BeforeProcessAsync``` method and add the engine configuration here.
Although it has a more general character, the changes here can only be overwritten either by another behavior with less priority
or with an explicit ```[Override]``` annotation.

### Example with lambda configuration

```C#
var processor = injector.Resolve<ITemplateProcessor>();
var result = await processor.ProcessAsync(
    new FileTemplate("C:\\Path\\To\\template.cshtml"),
    new TemplateModel("Johnny"),
    ctx => {
        PageDirective.Register(builder);
    });

Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);
```

### Example with a template processing behavior

```C#
[TemplateKind(RazorTemplatingEngine.Cshtml)]
public class AddPageTemplateProcessingBehavior : ITemplateProcessingBehavior
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
```

## Additional services
All the enumerated services provide a default implementation which can be overridden if required.
However, this should be rarely needed, instead configure the template generation

### ```IMetadataReferenceManager```
This internal service is used to provide ```MetadataReference```s out of a list of assemblies.

### ```IRazorProjectFileSystemProvider```
Based on the template and the processing context, this service should provide a ```RazorProjectFileSystem``` used by the renderer.

The default implementation ```SimpleRazorProjectFileSystemProvider``` uses a simple strategy which handles a single template file without any additional template references.

### ```IRazorProjectEngineFactory```
Creates the Razor project engine for the provided file system and with the given context.

### ```IRazorPageGenerator```
Generates the C# code based on the provided project engine and item.

### ```IRazorPageCompiler```
Compiles the template with model type ```T``` and returns a ```ICompiledRazorPage<T>``` wrapped into an operation result.