param (
    [string]$CertificateSubjectName = "KEPHAS SOFTWARE SRL",
    [string]$Timestamper = "http://timestamp.digicert.com"
)

# The symbol source is changed since Nov. 2018
# https://blog.nuget.org/20181116/Improved-debugging-experience-with-the-NuGet-org-symbol-server-and-snupkg.html
# however, a bug is still present - until it is fixed stay with the ols symbols


$packages = @(
    "Crc32C.NET.signed.1.0.5",
    "Snappy.NET.signed.1.1.1.8"
)

foreach ($package in $packages) {
    .\NuGet.exe sign "$package.nupkg" -CertificateSubjectName "$CertificateSubjectName" -Timestamper "$Timestamper"
}
