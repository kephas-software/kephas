param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release",
    [string]$apiKey = ""
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "")
}

$paths = @(
    "Kephas.Application",
    "Kephas.Application.AspNetCore",
    "Kephas.Application.Console",
    "Kephas.CodeAnalysis",
    "Kephas.Collections",
    "Kephas.Commands",
    "Kephas.Commands.Messaging",
    "Kephas.Composition.Autofac",
    "Kephas.Composition.Mef",
    "Kephas.Core",
    "Kephas.Core.Endpoints",
    "Kephas.Data",
    "Kephas.Data.Client",
    "Kephas.Data.Endpoints",
    "Kephas.Data.IO",
    "Kephas.Data.LLBLGen",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Extensions.Configuration",
    "Kephas.Extensions.DependencyInjection",
    "Kephas.Extensions.Hosting",
    "Kephas.Extensions.Logging",
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Serilog",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Messaging",
    "Kephas.Messaging.Model",
    "Kephas.Messaging.Pipes",
    "Kephas.Messaging.Redis",
    "Kephas.Model",
    "Kephas.Npgsql",
    "Kephas.Orchestration",
    "Kephas.Plugins",
    "Kephas.Plugins.Endpoints",
    "Kephas.Plugins.NuGet",
    "Kephas.Redis",
    "Kephas.Scheduling",
    "Kephas.Scheduling.Endpoints",
#    "Kephas.Scheduling.Quartz",
#    "Kephas.Scheduling.Quartz.MongoDB",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Scripting.Lua",
    "Kephas.Scripting.Python",
    "Kephas.Serialization.MongoDBBson",
    "Kephas.Serialization.NewtonsoftJson",
    "Kephas.TextProcessing",
    "Kephas.Workflow",
    "Kephas.Workflow.Model",
    "TestingFramework\Kephas.Testing",
    "TestingFramework\Kephas.Testing.Composition.Autofac",
    "TestingFramework\Kephas.Testing.Composition.Mef",
    "TestingFramework\Kephas.Testing.Model"
)

foreach ($path in $paths) {
    $packagename = get-packagename $path
    $packagepath = "..\$path\bin\$build\$packagename.$version.nupkg"
    if ($apiKey -eq "") {
        .\NuGet.exe push $packagepath 
    }
    else {
        .\NuGet.exe push -ApiKey $apiKey $packagepath 
    }
}