param (
    [string]$version = $( Read-Host "Please provide package version" )
)

$packages = @(
    "Kephas.Core",
    "Kephas.Configuration.Legacy",
    "Kephas.CodeAnalysis",
    "Kephas.Logging.NLog",
    "Kephas.Logging.Log4Net",
    "Kephas.Logging.Serilog",
    "Kephas.Composition.Autofac",
    "Kephas.Composition.Mef",
    "Kephas.Extensions.Configuration",
    "Kephas.Extensions.DependencyInjection",
    "Kephas.Messaging",
    "Kephas.Messaging.Model",
    "Kephas.Model",
    "Kephas.Data",
    "Kephas.Data.Client",
    "Kephas.Data.LLBLGen",
    "Kephas.Data.Model",
    "Kephas.Data.Model.Abstractions",
    "Kephas.Data.MongoDB",
    "Kephas.Data.IO",
    "Kephas.Data.Endpoints",
    "Kephas.Serialization.Json",
    "Kephas.Mail",
    "Kephas.Mail.MailKit",
    "Kephas.Npgsql",
    "Kephas.Plugins",
    "Kephas.ServiceStack",
    "Kephas.Serialization.ServiceStack.Text",
    "Kephas.Scripting",
    "Kephas.Scripting.CSharp",
    "Kephas.Orchestration",
    "Kephas.Application",
    "Kephas.Application.AspNetCore",
    "Kephas.Application.Console",
    "Kephas.Testing",
    "Kephas.Testing.Composition.Autofac",
    "Kephas.Testing.Composition.Mef",
    "Kephas.Testing.Model"
)

foreach ($package in $packages) {
    .\NuGet.exe push "$package.$version.nupkg"
}