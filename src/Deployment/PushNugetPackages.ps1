param (
    [string]$version = $( Read-Host "Please provide package version" )
)

$packages = @(
    "Kephas.Application",
    "Kephas.Application.AspNetCore",
    "Kephas.Application.Console",
    "Kephas.CodeAnalysis",
    "Kephas.Composition.Autofac",
    "Kephas.Composition.Mef",
    "Kephas.Configuration.Legacy",
    "Kephas.Core",
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
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Serilog",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Messaging",
#    "Kephas.Messaging.Azure.ServiceBus",
    "Kephas.Messaging.Model",
    "Kephas.Messaging.Redis",
    "Kephas.Model",
    "Kephas.Npgsql",
    "Kephas.Orchestration",
    "Kephas.Plugins",
    "Kephas.Redis",
    "Kephas.Scheduling",
#    "Kephas.Scheduling.Quartz",
#    "Kephas.Scheduling.Quartz.MongoDB",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Scripting.Lua",
    "Kephas.Scripting.Python",
    "Kephas.Serialization.Json",
    "Kephas.Serialization.ServiceStack.Text",
    "Kephas.ServiceStack",
    "Kephas.Workflow",
    "Kephas.Workflow.Model",
    "Kephas.Testing",
    "Kephas.Testing.Composition.Autofac",
    "Kephas.Testing.Composition.Mef",
    "Kephas.Testing.Model"
)

foreach ($package in $packages) {
    .\NuGet.exe push "$package.$version.nupkg"
}