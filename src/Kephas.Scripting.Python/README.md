# Python Scripting

## Introduction

The Python scripting area in Kephas handles Python dynamic code execution using [IronPython](https://www.nuget.org/packages/IronPython).

## Usage

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

// the next line uses Python scripting, so you will need also to reference Kephas.Scripting.Python.
var result = processor.ExecuteAsync(new PythonStringScript("name[..4] + str(age)"), new Expando { ["name"] = "Johnny", ["age"] = 42 }));
Assert.Equals("John42", result);
```

## The ```PythonLanguageService```
This service is the ```ILanguageService``` implementation for the Python language.
To minimize the load