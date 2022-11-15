# LUA Scripting

## Introduction

The LUA scripting area in Kephas handles LUA dynamic code execution using [NeoLua](https://www.nuget.org/packages/NeoLua).

Check the following packages for more information:
* [Kephas.Scripting](https://www.nuget.org/packages/Kephas.Scripting)
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

## Usage

* String based script execution

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

var script = new LuaStringScript(
@"function Power(a)
  return a * a
end

return Power(value) + 3");
var result = await processor.ExecuteAsync(script), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);
```

* File based script execution

The file scripts assume that the language can be inferred from the file extension.
For LUA, it means that the file needs to and with a *.lua extension.
If this is not the case, make sure you specify the language explicitly when creating a ```FileScript```.

```C#
// normally you would get the processor injected into the service constructor.
var processor = injector.Resolve<IScriptProcessor>();

// LUA scripts ending with *.lua
var result = await processor.ExecuteAsync(new FileScript("myscript.lua"), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);

// LUA scripts not ending with *.lua
var result = await processor.ExecuteAsync(new FileScript("myscript.txt", LuaLanguageService.Language), new { value = 5 })).PreserveThreadContext();
Assert.Equals(28, result);
```
