# Python Scripting

## Introduction

The Python scripting area in Kephas handles Python dynamic code execution using [IronPython](https://www.nuget.org/packages/IronPython).

Check the following packages for more information:
* [Kephas.Scripting](https://www.nuget.org/packages/Kephas.Scripting)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Configuration](https://www.nuget.org/packages/Kephas.Configuration)

## Usage

* String based script execution

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();
var result = await processor.ExecuteAsync(new PythonStringScript("name[..4] + str(age)"), new Expando { ["name"] = "Johnny", ["age"] = 42 })).PreserveThreadContext();
Assert.Equals("John42", result);
```

* File based script execution

The file scripts assume that the language can be inferred from the file extension.
For Python, it means that the file needs to and with a *.py extension.
If this is not the case, make sure you specify the language explicitly when creating a ```FileScript```.

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

// Python scripts ending with *.py
var result = await processor.ExecuteAsync(new FileScript("myscript.py"), new { name = "Johnny", age = 42 })).PreserveThreadContext();
Assert.Equals("John42", result);

// Python scripts not ending with *.py
var result = await processor.ExecuteAsync(new FileScript("myscript.txt", PythonLanguageService.Language), new { name = "Johnny", age = 42 })).PreserveThreadContext();
Assert.Equals("John42", result);
```

## The ```PythonLanguageService```
This service is the ```ILanguageService``` implementation for the Python language.
It uses the ```PythonSettings``` to configure the way it controls the execution. These are the options:
* _SearchPaths_ (string[]): enumerates the paths where the Python modules should be searched.
* _PreloadGlobalModules_ (boolean): Indicates whether the modules found in the search paths should be preloaded for the executing scripts. This way, the scripts would be simpler to write, but the resource consumption could be greater.

> Note: The search paths are locations handled by the ```ILocationsManager``` service.
> When adding tenant support, each tenant will get its own copy.

### Configuration file example:

```json
{
    "searchPaths": [ "../config/.pylib" ],
    "preloadGlobalModules": true
}
```