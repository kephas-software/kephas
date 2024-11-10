# Security

## Introduction

This package provides abstractions and base building blocks for authentication, authorization, and cryptography.
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)

Packages providing advanced cryptography:
* [Kephas.Security.Cryptography](https://www.nuget.org/packages/Kephas.Security.Cryptography)

Typically used areas and classes/interfaces/services:
* Authentication: ```IAuthenticationService```.
* Authorization: ```IAuthorizationService```.
* Cryptography: ```IEncryptionService```, ```IHashingService```.
* Permissions: ```IPermissionInfo```.

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

The authorization service is used to ensure that a certain context has the required authorization.

### IAuthorizationService

This is a singleton application service contract providing a single method:

* `AuthorizeAsync(authContext: IAuthorizationContext, cancellationToken: CancellationToken = default): Task<bool>`

### IAuthorizationContext

An authorization context contains:
* _Identity_ (inherited from the base `IContext`): the identity requesting authorization.
* _RequiredPermissions_/_RequiredPermissionTypes_: the list of permissions to check.
* _Scope_ (optional): a scope object for which the required permissions apply.
* _ThrowOnFailure_ (default `true`): A boolean value indicating whether to throw on authorization failure. If <c>false</c> is indicated, the authorization check will return <c>false</c> upon failure, otherwise an exception will occur.

## The permission system

Data must be protected from unauthorized access for different kind of reasons. Kephas brings the required support at multiple levels providing built-in services supporting multiple authorization scenarios.

## Permissions

Permissions are basically string tokens required by certain operations in a given context. Permissions:
* may use an "inheritance" model, with the meaning that if a permission inherits another permission, both of them are granted to the role associated to them.
* can be scoped to entity hierarchies and further to entity sections, meaning that they are granted only within that specific scope.

Permissions have associated metadata collected by the model space. They may be defined using interfaces with multiple inheritance, or (abstract) classes annotated with `[GrantPermission]` attributes. To define custom permissions, use the following steps:

1. Define the type holding the permission metadata.
```C#
[PermissionType("admin")]
public interface IAdminPermission : ICrudPermission, IExportImportPermission
{
}

// alternative way using abstract classes.
[PermissionType("admin")]
[GrantsPermission(typeof(CrudPermission), typeof(ExportImportPermission))]
public abstract class AdminPermission
{
}
```

2. Annotate the assembly/namespace containing the definitions with `[PermissionAssembly]` attribute.
```C#
[assembly: PermissionAssembly("MyApp.Security.Permissions")]
```

3. Use the permission using its .NET type, typically in `[RequiresPermission]` or `[SupportsPermission]` attributes. Alternatively, such attributes support also permission names (strings), but it is not that safe for refactorings.
```C#
/// <summary>
/// An export hierarchy message.
/// </summary>
[RequiresPermission(typeof(IExportImportPermission))]
public class ExportHierarchyMessage : EntityActionMessage
{
    /// <summary>
    /// Gets or sets the export media type to use.
    /// </summary>
    /// <value>
    /// The export media type.
    /// </value>
    public string MediaType { get; set; }
}
```

> Note: It may be more practical to use interfaces, because this way the inheritance hierarchy can be displayed in a class diagram. Anyway, the interface inheritance model and the grants model can be combined, having the same effect.

## Scoping permissions

Permissions may indicate a certain application scope. This can be:

* `Global`: No scoping required for this permission type, it will be granted and verified at global level.
* `Type`: The scope for this permission is the entity type.
* `Instance`: The scope for this permission is the entity instance.

These values are flags which can be combined to provide multiple supported scenarios for a specific permission type.