# Scripting

## Introduction

The scripting area in Kephas handles dynamic code execution.
The entry point is the ```IScriptProcessor``` singleton service, which returns a result through the ```ExecuteAsync``` method, provided with a script, execution arguments, and globals.

## Usage

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

// the next line uses C# scripting, so you will need also to reference Kephas.Scripting.CSharp.
var result = processor.ExecuteAsync(new CSharpStringScript("name[..4] + age.ToString()"), new { name = "Johnny", age = 42 }));
Assert.Equals("John42", result);

// the next line uses Python scripting, so you will need also to reference Kephas.Scripting.Python.
// additionally, instead of using typed arguments, it used an Expando - it could use as well a IDictionary<string, object?>.
var result = processor.ExecuteAsync(new PythonStringScript("name[..4] + str(age)"), new Expando { ["name"] = "Johnny", ["age"] = 42 }));
Assert.Equals("John42", result);
```

## The ```IScriptProcessor``` service
This service is the central piece providing the ```ExecuteAsync``` method through which a script is executed.
The script return value is, in turn, returned by the method.

```DefaultScriptProcessor``` is the default implementation of the ```IScriptProcessor```.
Just like the other default implementations provided by the Kephas framework, it declares a low override priority, so that it can be easily overridden.
By default, it delegates the template processing to a specific ```ILanguageService``` handling the provided script.
It identifies the language service based on the script's language, which the service declares using the ```[Language(...)]``` attribute.

### Passing arguments

The arguments passed to the execution can be referenced in the scripts directly by their name.
However, if the ```DeconstructArgs``` scripting context options is set to false, the arguments can be accessed through the ```Args``` global variable.

#### Example

```c#
var processor = injector.Resolve<IScriptProcessor>();
var script = new CSharpStringScript(
    "int Power(int a) => a * a;" +
    "return Power((int)Args.a);");
var result = await processor.ExecuteAsync(script, new { a = 2 }, ctx => ctx.DeconstructArgs(false)); 
```

## Language services
These services implement the ```ILanguageService``` contract and handle specific languages.
By default, the framework does not provide any language service in the ```Kephas.Scripting``` package due to the heavy overhead it brings.
However, there are several language services provided in separate packages:
* [Kephas.Scripting.CSharp](https://www.nuget.org/packages/Kephas.Scripting.CSharp)
* [Kephas.Scripting.Python](https://www.nuget.org/packages/Kephas.Scripting.Python)
* [Kephas.Scripting.Lua](https://www.nuget.org/packages/Kephas.Scripting.Lua)
## Controlling the script execution

### The scripting context
Additional to the script and the arguments, the processing methods receive also a context which can be used to further control the operations.
Just like all the other context instances, it is a dynamic expandable object (```IExpando```) and it aggregates the provided template and the model.
Through the ```Result``` and ```Exception``` properties, the behaviors (see below) can control the processing output.

### The scripting behaviors
The ```DefaultScriptProcessor``` uses also ```IScriptingBehavior``` services to further control the execution. Just like the ```ILanguageService``` services, they declare the language they support,
and are invoked before and after the selected language service executes the script. They can cover various purposes: authorization, pre-processing/post-processing, audit, and whatever other functionality may be required.

## The scripts
A script implements ```IScript```, basically providing a ```Language``` (string), a ```Name``` (string), and source code (```GetSourceCodeAsync()```, ```GetSourceCode()```).
By default, the framework supports these script:
* ```StringScript```: constructed using a string.
* ```StreamScript```: constructed using a ```Stream```.
* ```FileScript```: constructed using a file path. If not provided, the ```Language```  is considered to be the file extension.
