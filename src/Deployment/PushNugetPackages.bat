@echo off
@if [%1]==[] goto usage

@echo on
nuget push Kephas.Core.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Localization.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Logging.NLog.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Logging.Log4Net.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Composition.Mef.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Platform.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Model.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.Client.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.Model.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.InMemory.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.IO.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Data.Endpoints.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Serialization.Json.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Serialization.ServiceStack.Text.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Messaging.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Messaging.Model.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.CodeAnalysis.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Mail.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Mail.MailKit.%1.nupkg -Source https://www.nuget.org/api/v2/package
rem nuget push Kephas.Mail.Services.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Web.Owin.%1.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Kephas.Npgsql.%1.nupkg -Source https://www.nuget.org/api/v2/package

@echo Done.
@pause

@goto :eof
:usage
@echo Usage: %0 ^<version-number^>

:eof
@pause
@exit /B 1