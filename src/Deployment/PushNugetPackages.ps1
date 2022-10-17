param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release",
    [string]$apiKey = "",
    [string]$singlePackage = ""
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "").Replace("Analyzers\", "")
}

$paths =
If ([string]::IsNullOrEmpty($singlePackage))
{ @(
    "Kephas.Abstractions",
    "Kephas.Application",
    "Kephas.Application.Abstractions",
    "Kephas.Application.Console",
    "Kephas.AspNetCore",
    "Kephas.AspNetCore.IdentityServer4",
    "Kephas.AspNetCore.SignalR",
    "Kephas.Behaviors",
    "Kephas.CodeAnalysis",
    "Kephas.Collections",
    "Kephas.Commands",
    "Kephas.Commands.Endpoints.Messages",
    "Kephas.Commands.Messaging",
    "Kephas.Configuration",
    "Kephas.Connectivity",
    "Kephas.Core",
    "Kephas.Core.Endpoints",
    "Kephas.Core.Endpoints.Messages",
    "Kephas.Data",
    "Kephas.Data.Abstractions",
    "Kephas.Data.Client",
    "Kephas.Data.Endpoints",
    "Kephas.Data.Endpoints.Messages",
    "Kephas.Data.IO",
    "Kephas.Data.LLBLGen",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Data.Redis",
    "Kephas.Diagnostics.Contracts",
    "Kephas.Extensions.Configuration",
    "Kephas.Extensions.DependencyInjection",
    "Kephas.Extensions.Hosting",
    "Kephas.Extensions.Logging",
    "Kephas.Injection",
    "Kephas.Injection.Autofac",
    "Kephas.Injection.Lite",
    "Kephas.Interaction",
    "Kephas.Licensing",
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
    "Kephas.MongoDB",
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
    "Kephas.Templating",
    "Kephas.Templating.Razor",
    "Kephas.Tenants",
    "Kephas.TextProcessing",
    "Kephas.Validation",
    "Kephas.Versioning",
    "Kephas.Workflow",
    "Kephas.Workflow.Abstractions",
    "Kephas.Workflow.Model",
    "Analyzers\Kephas.Analyzers",
    "TestingFramework\Kephas.Testing",
    "TestingFramework\Kephas.Testing.Application",
    "TestingFramework\Kephas.Testing.Model"
) }
Else
{ @("$singlePackage") }

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