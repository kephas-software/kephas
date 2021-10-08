param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release",
    [string]$CertificateSubjectName = "Kephas Software SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "").Replace("Analyzers\", "")
}

$paths = @(
    "..\Kephas.Application",
    "..\Kephas.Application.Console",
    "..\Kephas.AspNetCore",
    "..\Kephas.AspNetCore.IdentityServer4",
    "..\Kephas.AspNetCore.SignalR",
    "..\Kephas.CodeAnalysis",
    "..\Kephas.Collections",
    "..\Kephas.Commands",
    "..\Kephas.Commands.Messaging",
    "..\Kephas.Core",
    "..\Kephas.Core.Abstractions",
    "..\Kephas.Core.Endpoints",
    "..\Kephas.Core.Endpoints.Messages",
    "..\Kephas.Data",
    "..\Kephas.Data.Client",
    "..\Kephas.Data.Endpoints",
    "..\Kephas.Data.IO",
    "..\Kephas.Data.LLBLGen",
    "..\Kephas.Data.Model",
    "..\Kephas.Data.Model.Abstractions",
    "..\Kephas.Data.MongoDB",
    "..\Kephas.Extensions.Configuration",
    "..\Kephas.Extensions.DependencyInjection",
    "..\Kephas.Extensions.Hosting",
    "..\Kephas.Extensions.Logging",
    "..\Kephas.Injection.Abstractions",
    "..\Kephas.Injection.Autofac",
    "..\Kephas.Injection.Lite",
    "..\Kephas.Injection.SystemComposition",
    "..\Kephas.Logging.Log4Net",
    "..\Kephas.Logging.NLog",
    "..\Kephas.Logging.Serilog",
    "..\Kephas.Mail",
    "..\Kephas.Mail.MailKit",
    "..\Kephas.Messaging",
    "..\Kephas.Messaging.Distributed",
    "..\Kephas.Messaging.Distributed.Pipes",
    "..\Kephas.Messaging.Distributed.Redis",
    "..\Kephas.Messaging.Messages",
    "..\Kephas.Messaging.Model",
    "..\Kephas.Model",
    "..\Kephas.Npgsql",
    "..\Kephas.Orchestration",
    "..\Kephas.Plugins",
    "..\Kephas.Plugins.Abstractions",
    "..\Kephas.Plugins.Endpoints",
    "..\Kephas.Plugins.Endpoints.Messages",
    "..\Kephas.Plugins.NuGet",
    "..\Kephas.Redis",
    "..\Kephas.Scheduling",
    "..\Kephas.Scheduling.Endpoints",
    "..\Kephas.Scheduling.Quartz",
    "..\Kephas.Scheduling.Quartz.MongoDB",
    "..\Kephas.Scripting",
    "..\Kephas.Scripting.CSharp",
    "..\Kephas.Scripting.Lua",
    "..\Kephas.Scripting.Python",
    "..\Kephas.Security.Cryptography",
    "..\Kephas.Security.Permissions",
    "..\Kephas.Serialization.MongoDBBson",
    "..\Kephas.Serialization.NewtonsoftJson",
    "..\Kephas.TextProcessing",
    "..\Kephas.Workflow",
    "..\Kephas.Workflow.Model",
    "..\Analyzers\Kephas.Analyzers",
    "..\TestingFramework\Kephas.Testing",
    "..\TestingFramework\Kephas.Testing.Application",
    "..\TestingFramework\Kephas.Testing.Injection.Autofac",
    "..\TestingFramework\Kephas.Testing.Injection.SystemComposition",
    "..\TestingFramework\Kephas.Testing.Model"
)

foreach ($path in $paths) {
    $packagename = get-packagename $path
    $packagepath = "$path\bin\$build\$packagename.$version.nupkg"
    $symbolspackagepath = "$path\bin\$build\$packagename.$version.snupkg"
    .\NuGet.exe sign "$packagepath" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
    .\NuGet.exe sign "$symbolspackagepath" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"

    # do not sign the symbols.nupkg package as after renamig them for some older nuget servers like ProGet, they are not valid anymore
    # .\NuGet.exe sign "$packagename.$version.symbols.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
}
