param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$refversion = $( Read-Host "Please provide the referenced package version" ),
    [string]$build = "Debug",
    [string]$CertificateSubjectName = "CRIŞAN IOAN SOFT PERSOANĂ FIZICĂ AUTORIZATĂ",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

function get-packagename([string]$pathname) {
    return $pathname.Replace("..\", "")
}

$paths = @(
    "..\Kephas.Core",
    "..\Kephas.Logging.NLog",
    "..\Kephas.Logging.Log4Net",
    "..\Kephas.Composition.Mef",
    "..\Kephas.Messaging",
    "..\Kephas.Messaging.Model",
    "..\Kephas.Model",
    "..\Kephas.Data",
    "..\Kephas.Data.Client",
    "..\Kephas.Data.Entity",
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
    "..\Kephas.ServiceStack",
    "..\Kephas.Serialization.ServiceStack.Text",
    "..\Kephas.Scripting",
    "..\Kephas.Scripting.CSharp",
    "..\Kephas.Orchestration",
    "..\Kephas.AspNetCore"
)

foreach ($path in $paths) {
    .\NuGet.exe pack $path\Package.nuspec -BasePath $path -Symbols -properties "Configuration=$build;Version=$version;RefVersion=$refversion"
    $packagename = get-packagename $path
    .\NuGet.exe sign "$packagename.$version.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
}
