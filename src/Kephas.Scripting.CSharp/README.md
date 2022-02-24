# C# Scripting

## Introduction

The C# scripting area in Kephas handles C# dynamic code execution using [Microsoft.CodeAnalysis.CSharp.Scripting](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Scripting).

Check the following packages for more information:
* [Kephas.Scripting](https://www.nuget.org/packages/Kephas.Scripting)
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

## Usage

* String based script execution

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

var script = new CSharpStringScript(
@"int Power(int a) => a * a;
return Power((int)value) + 3;
");
var result = await processor.ExecuteAsync(script), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);
```

* File based script execution

The file scripts assume that the language can be inferred from the file extension.
For C#, it means that the file needs to and with a *.cs or *.csx extension.
If this is not the case, make sure you specify the language explicitly when creating a ```FileScript```.

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

// C# scripts ending with *.csx
var result = await processor.ExecuteAsync(new FileScript("myscript.csx"), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);

// C# scripts not ending with *.csx or *.cs
var result = await processor.ExecuteAsync(new FileScript("myscript.txt", CSharpLanguageService.Language), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);
```
