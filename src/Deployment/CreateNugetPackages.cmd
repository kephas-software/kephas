@echo off
if [%1]==[] goto usage
if [%2]==[] goto usage
set BuildConfiguration=%3
if "%BuildConfiguration%"=="" set BuildConfiguration=Debug

set PackageVersion=%1
set PackageRefVersion=%2

@echo on

nuget pack ..\Kephas.Core\Package.nuspec -BasePath ..\Kephas.Core -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Localization\Package.nuspec -BasePath ..\Kephas.Localization -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Logging.NLog\Package.nuspec -BasePath ..\Kephas.Logging.NLog -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Logging.Log4Net\Package.nuspec -BasePath ..\Kephas.Logging.Log4Net -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Composition.Mef\Package.nuspec -BasePath ..\Kephas.Composition.Mef -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Messaging\Package.nuspec -BasePath ..\Kephas.Messaging -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Messaging.Model\Package.nuspec -BasePath ..\Kephas.Messaging.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Model\Package.nuspec -BasePath ..\Kephas.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data\Package.nuspec -BasePath ..\Kephas.Data -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.Client\Package.nuspec -BasePath ..\Kephas.Data.Client -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.Model\Package.nuspec -BasePath ..\Kephas.Data.Model -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.MongoDB\Package.nuspec -BasePath ..\Kephas.Data.MongoDB -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.InMemory\Package.nuspec -BasePath ..\Kephas.Data.InMemory -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.IO\Package.nuspec -BasePath ..\Kephas.Data.IO -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Data.Endpoints\Package.nuspec -BasePath ..\Kephas.Data.Endpoints -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Serialization.Json\Package.nuspec -BasePath ..\Kephas.Serialization.Json -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Platform\Package.nuspec -BasePath ..\Kephas.Platform -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.CodeAnalysis\Package.nuspec -BasePath ..\Kephas.CodeAnalysis -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Mail\Package.nuspec -BasePath ..\Kephas.Mail -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Mail.MailKit\Package.nuspec -BasePath ..\Kephas.Mail.MailKit -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Mail.Services.Net45\Package.nuspec -BasePath ..\Kephas.Mail.Services.Net45 -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Kephas.Web.Owin\Package.nuspec -BasePath ..\Kephas.Web.Owin -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%
nuget pack ..\Adapters\Kephas.Npgsql\Package.nuspec -BasePath ..\Adapters\Kephas.Npgsql -Symbols -properties Configuration=%BuildConfiguration%;Version=%PackageVersion%;RefVersion=%PackageRefVersion%

@echo Done.
@pause

@goto :eof
:usage
@echo Usage: %0 ^<version^> ^<ref-version^> [^<build-configuration^> = Debug]
@echo version is the package version number
@echo ref-version is the version of the other referenced Kephas packages
@echo build-configuration is the build configuration used for packaging (default is Debug)

:eof
@pause
@exit /B 1