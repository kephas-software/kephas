param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release",
    [string]$apiKey = ""
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "").Replace("Analyzers\", "")
}

$paths = @(
    "Kephas.Application",
    "Kephas.Application.Abstractions",
    "Kephas.Application.Console",
    "Kephas.AspNetCore",
    "Kephas.AspNetCore.IdentityServer4",
    "Kephas.AspNetCore.SignalR",
    "Kephas.CodeAnalysis",
    "Kephas.Collections",
    "Kephas.Commands",
    "Kephas.Commands.Endpoints.Messages",
    "Kephas.Commands.Messaging",
    "Kephas.Configuration",
    "Kephas.Core",
    "Kephas.Core.Abstractions",
    "Kephas.Core.Endpoints",
    "Kephas.Core.Endpoints.Messages",
    "Kephas.Data",
    "Kephas.Data.Client",
    "Kephas.Data.Endpoints",
    "Kephas.Data.Endpoints.Messages",
    "Kephas.Data.IO",
    "Kephas.Data.LLBLGen",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Extensions.Configuration",
    "Kephas.Extensions.DependencyInjection",
    "Kephas.Extensions.Hosting",
    "Kephas.Extensions.Logging",
    "Kephas.Injection",
    "Kephas.Injection.Autofac",
    "Kephas.Injection.Lite",
#    "Kephas.Injection.SystemComposition",
    "Kephas.Interaction",
    "Kephas.Logging",
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Serilog",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Messaging",
    "Kephas.Messaging.Distributed",
    "Kephas.Messaging.Distributed.Pipes",
    "Kephas.Messaging.Distributed.Redis",
    "Kephas.Messaging.Messages",
    "Kephas.Messaging.Model",
    "Kephas.Model",
    "Kephas.Npgsql",
    "Kephas.Operations",
    "Kephas.Orchestration",
    "Kephas.Plugins",
    "Kephas.Plugins.Abstractions",
    "Kephas.Plugins.Endpoints",
    "Kephas.Plugins.Endpoints.Messages",
    "Kephas.Plugins.NuGet",
    "Kephas.Redis",
    "Kephas.Reflection",
    "Kephas.Reflection.Dynamic",
    "Kephas.Scheduling",
    "Kephas.Scheduling.Abstractions",
    "Kephas.Scheduling.Endpoints",
    "Kephas.Scheduling.Endpoints.Messages",
#    "Kephas.Scheduling.Quartz",
#    "Kephas.Scheduling.Quartz.MongoDB",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Scripting.Lua",
    "Kephas.Scripting.Python",
    "Kephas.Security",
    "Kephas.Security.Cryptography",
    "Kephas.Security.Permissions",
    "Kephas.Serialization",
    "Kephas.Serialization.MongoDBBson",
    "Kephas.Serialization.NewtonsoftJson",
    "Kephas.TextProcessing",
    "Kephas.Workflow",
    "Kephas.Workflow.Abstractions",
    "Kephas.Workflow.Model",
    "Analyzers\Kephas.Analyzers",
    "TestingFramework\Kephas.Testing",
    "TestingFramework\Kephas.Testing.Application",
    "TestingFramework\Kephas.Testing.Injection.Autofac",
#    "TestingFramework\Kephas.Testing.Injection.SystemComposition",
    "TestingFramework\Kephas.Testing.Model"
)

foreach ($path in $paths) {
    $packagename = get-packagename $path
    $packagepath = "..\$path\bin\$build\$packagename.$version.nupkg"
    if ($apiKey -eq "") {
        .\NuGet.exe push $packagepath -Source https://api.nuget.org/v3/index.json
    }
    else {
        .\NuGet.exe push $packagepath -Source https://api.nuget.org/v3/index.json -ApiKey $apiKey
    }
}