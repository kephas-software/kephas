param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Release",
    [string]$CertificateSubjectName = "Kephas Software SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "")
}

$paths = @(
    "..\Kephas.Application",
    "..\Kephas.Application.AspNetCore",
    "..\Kephas.Application.Console",
    "..\Kephas.CodeAnalysis",
    "..\Kephas.Collections",
    "..\Kephas.Composition.Autofac",
    "..\Kephas.Composition.Mef",
    "..\Kephas.Configuration.Legacy",
    "..\Kephas.Core",
    "..\Kephas.Core.Endpoints",
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
    "..\Kephas.Logging.Log4Net",
    "..\Kephas.Logging.NLog",
    "..\Kephas.Logging.Serilog",
    "..\Kephas.Mail",
    "..\Kephas.Mail.MailKit",
    "..\Kephas.Messaging",
    "..\Kephas.Messaging.Model",
    "..\Kephas.Messaging.Redis",
    "..\Kephas.Model",
    "..\Kephas.Npgsql",
    "..\Kephas.Orchestration",
    "..\Kephas.Plugins",
    "..\Kephas.Plugins.Endpoints",
    "..\Kephas.Plugins.NuGet",
    "..\Kephas.Redis",
    "..\Kephas.Scheduling",
    "..\Kephas.Scheduling.Quartz",
    "..\Kephas.Scheduling.Quartz.MongoDB",
    "..\Kephas.Scripting",
    "..\Kephas.Scripting.CSharp",
    "..\Kephas.Scripting.Lua",
    "..\Kephas.Scripting.Python",
    "..\Kephas.Serialization.NewtonsoftJson",
    "..\Kephas.TextProcessing",
    "..\Kephas.Workflow",
    "..\Kephas.Workflow.Model",
    "..\TestingFramework\Kephas.Testing",
    "..\TestingFramework\Kephas.Testing.Composition.Autofac",
    "..\TestingFramework\Kephas.Testing.Composition.Mef",
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
