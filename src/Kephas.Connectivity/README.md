# Connectivity

## Introduction

The purpose of this library is to provide abstractions for connections to various services.

Check the following packages for more information:
* [Kephas.Core](https://www.nuget.org/packages/Kephas.Core)
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Security](https://www.nuget.org/packages/Kephas.Security)

Packages providing connections:
* [Kephas.Mail.MailKit](https://www.nuget.org/packages/Kephas.Mail.MailKit)

Typically used areas and classes/interfaces/services:
* ```IConnection```, ```IConnectionProvider```, ```IConnectionFactory```.

## Usage

```C#
// this code snippet uses the Kephas.Mail.MailKit package. 
// normally you would get the processor injected into the service constructor.
var provider = injector.Resolve<IConnectionProvider>();
using var connection = await provider.CreateConnection(new Uri("imap://my.server.com:993/john.doe"), new UserClearTextPasswordCredentials("john", "doe"));
await connection.OpenAsync().PreserveThreadContext();

// do somethig with the IMAP connection to the email account.
// ...

```
