param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$refversion = $( Read-Host "Please provide the referenced package version" ),
    [string]$build = "Debug",
    [string]$CertificateSubjectName = "Kephas Software SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

# The symbol source is changed since Nov. 2018
# https://blog.nuget.org/20181116/Improved-debugging-experience-with-the-NuGet-org-symbol-server-and-snupkg.html
# however, a bug is still present - until it is fixed stay with the old symbols


function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "").Replace("TestingFramework\", "")
}

$paths = @(
    "..\Kephas.Core",
    "..\Kephas.Configuration.Legacy",
    "..\Kephas.Logging.NLog",
    "..\Kephas.Logging.Log4Net",
    "..\Kephas.Logging.Serilog",
    "..\Kephas.Composition.Autofac",
    "..\Kephas.Composition.Mef",
    "..\Kephas.Extensions.Configuration",
    "..\Kephas.Extensions.DependencyInjection",
    "..\Kephas.Messaging",
    "..\Kephas.Messaging.Model",
    "..\Kephas.Messaging.Azure.ServiceBus",
    "..\Kephas.Model",
    "..\Kephas.Data",
    "..\Kephas.Data.Client",
    "..\Kephas.Data.LLBLGen",
    "..\Kephas.Data.Model",
    "..\Kephas.Data.Model.Abstractions",
    "..\Kephas.Data.MongoDB",
    "..\Kephas.Data.IO",
    "..\Kephas.Data.Endpoints",
    "..\Kephas.Serialization.Json",
    "..\Kephas.CodeAnalysis",
    "..\Kephas.Mail",
    "..\Kephas.Mail.MailKit",
    "..\Kephas.Npgsql",
    "..\Kephas.Plugins",
    "..\Kephas.ServiceStack",
    "..\Kephas.Serialization.ServiceStack.Text",
    "..\Kephas.Scripting",
    "..\Kephas.Scripting.CSharp",
    "..\Kephas.Orchestration",
    "..\Kephas.Application",
    "..\Kephas.Application.AspNetCore",
    "..\Kephas.Application.Console",
    "..\Kephas.Workflow",
    "..\Kephas.Workflow.Model",
    "..\TestingFramework\Kephas.Testing",
    "..\TestingFramework\Kephas.Testing.Composition.Autofac",
    "..\TestingFramework\Kephas.Testing.Composition.Mef",
    "..\TestingFramework\Kephas.Testing.Model"
)

foreach ($path in $paths) {
    .\NuGet.exe pack $path\Package.nuspec -BasePath $path -Symbols <# -SymbolPackageFormat snupkg #> -properties "Configuration=$build;Version=$version;RefVersion=$refversion"
    .\NuGet.exe pack $path\Package.nuspec -BasePath $path -Symbols -SymbolPackageFormat snupkg -properties "Configuration=$build;Version=$version;RefVersion=$refversion"
    $packagename = get-packagename $path
    .\NuGet.exe sign "$packagename.$version.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
#    .\NuGet.exe sign "$packagename.$version.symbols.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
    .\NuGet.exe sign "$packagename.$version.snupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
}
