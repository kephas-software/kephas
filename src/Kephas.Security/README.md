# Security

## Introduction

This package provides abstractions and base building blocks for authentication, authorization, and cryptography.
* [Kephas.Injection](https://www.nuget.org/packages/Kephas.Injection)

Packages providing advanced cryptography:
* [Kephas.Security.Cryptography](https://www.nuget.org/packages/Kephas.Security.Cryptography)

## Cryptography

### The encryption service

#### Usage

```c#
// normally you would get the encryption service injected into the service constructor.
var encryptionService = injector.Resolve<IEncryptionService>();
var encrypted = encryptionService.Encrypt("my-password");
var decrypted = encryptionService.Decrypt(encrypted);

Assert.AreEqual("my-password", decrypted);
```

### The hashing service

## Authentication

### The authentication service

## Authorization

### The authorization service

## Permissions
