# Templating.Razor

## Introduction
This package provides the implementation of templating using Razor syntax and *.cshtml files.
The ```cshtml``` template kind is handled by the ```RazorTemplatingEngine```.

## Usage

This is the content of the ```template.cshtml``` file.
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

```C#
record TemplateModel(string Name);

// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<ITemplateProcessor>();

var result = processor.Process(new FileTemplate("C:\\Path\\To\\template.cshtml"), new TemplateModel("Johnny"));
Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);

// this is a simpler alternative for Razor file templates.
var result = processor.ProcessWithFile("C:\\Path\\To\\template.cshtml", new TemplateModel("Johnny"));
Assert.Equals("<div>Hi @Johnny@!</div>\r\n", result);

// it is also possible to use string templates.
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