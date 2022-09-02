# Templating

## Introduction

The templating area in Kephas handles the text generation out of various templates.
The entry point is the ```ITemplateProcessor``` singleton service, which returns a processed result, typically in form of a ```string```, through the ```ProcessAsync``` method provided with a template and a model.

Check the following packages for more information:
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)
* [Kephas.Templating.Razor](https://www.nuget.org/packages/Kephas.Templating.Razor)

Typically used areas and classes/interfaces/services:
* ```ITemplateProcessor```, ```ITemplateProcessingBehavior```, ```ITemplatingEngine```.
* AttributedModel: ```TemplateKindAttribute```.

## Usage

```c#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<ITemplateProcessor>();

var result = processor.Process(new StringTemplate("hi {name}!", "interpolate"), new { name = "Johnny" }));
Assert.Equals("hi Johnny!", result);

// this is a simpler alternative for interpolation using the Interpolate extension method.
var result = processor.Interpolate("hi {name}!", new { name = "Johnny" }));
Assert.Equals("hi Johnny!", result);
```

## The ```ITemplateProcessor``` service
This service is the central piece providing the ```ProcessAsync``` method through which a template is processed.
Typically, the output is written to the ```ITemplateProcessingContext.TextWriter```, but, if this is not set by the user,
it is the duty of the template processor to prepare the ```TextWriter``` and to return in the operation result
the result returned by the processing engine.

```DefaultTemplateProcessor``` is the default implementation of the ```ITemplateProcessor```.
Just like the other default implementations provided by the Kephas framework, it declares a low override priority, so that it can be easily overridden.
By default, it delegates the template processing to a specific ```ITemplatingEngine``` handling the provided template.
It identifies the engine based on the template's kind, which the engine declares using the ```[TemplateKind(...)]``` attribute.

## Template engines
These services implement the ```ITemplatingEngine``` contract and handle specific template kinds.
By default, the framework provides the ```InterpolationTemplatingEngine``` which handles simple string interpolations using the curly braces pattern: ```{variable}```. All such variables are replaced with the values of the properties' with the same name in the model.

## Controlling the template processing

### The processing context
Additional to the template and the model, the processing methods receive also a context which can be used to further control the operations.
Just like all the other context instances, it is a dynamic expandable object (```IExpando```) and it aggregates the provided template and the model. 
Through the ```Result``` and ```Exception``` properties, the behaviors (see below) can control the processing output.

### The processing behaviors
The ```DefaultTemplateProcessor``` uses also ```ITemplateProcessingBehavior``` services to further control the processing. Just like the ```ITemplateEngine``` services, they declare the template kinds they handle,
and are invoked before and after the selected engine processes the template. They can cover various purposes: authorization, pre-processing/post-processing, audit, and whatever other functionality may be required.

## The templates
A template implements ```ITemplate```, basically providing a ```Kind``` (string), a ```Name``` (string), and an asynchronous content (```GetContentAsync()```).
By default, the framework supports these templates:
* ```StringTemplate```: constructed using a string. If the ```Kind``` is not provided, ```'interpolation'``` is considered instead.
* ```StringBuilderTemplate```: constructed using a ```StringBuilder```. If the ```Kind``` is not provided, ```'interpolation'``` is considered instead.
* ```StreamTemplate```: constructed using a ```Stream```. If the ```Kind``` is not provided, ```'interpolation'``` is considered instead.
* ```FileTemplate```: constructed using a file path. If the ```Kind``` is not provided, the file extension is considered instead.
