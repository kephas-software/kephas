param (
    [string]$version = $( Read-Host "Please provide package version" ),
    [string]$build = "Debug",
    [string]$CertificateSubjectName = "CRIŞAN IOAN SOFT PERSOANĂ FIZICĂ AUTORIZATĂ",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

# The symbol source is changed since Nov. 2018
# https://blog.nuget.org/20181116/Improved-debugging-experience-with-the-NuGet-org-symbol-server-and-snupkg.html
# however, a bug is still present - until it is fixed stay with the ols symbols


$packages = @(
    "MongoDB.Bson.signed",
    "MongoDB.Driver.Core.signed",
    "MongoDB.Driver.signed",
    "MongoDB.Driver.GridFS.signed"
)

foreach ($package in $packages) {
    .\NuGet.exe sign "$package.$version.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
}
