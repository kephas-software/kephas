# Tenants

## Introduction
This package adds support for multi-tenant applications.

How it works:
* Include the ```Kephas.Tenants``` package in the application deployment.
* The application receives the tenant identifier through the ```tenant``` command line argument or the ```KEPHASAPP_Tenant``` environment variable.
* The tenant is available in code through ```IAppArgs.Tenant()``` extension method or the ```IContext.Tenant()``` extension method.
  * Note that ```IAppArgs``` are available through dependency injection and ```IContext``` instances through the parameter received in context dependent invocations. 
* When setting up the ```IAmbientServices``` during application bootstrapping, make sure to invoke the ```ambientServices.WithTenantSupoport()```.
  * Do this typically **before** setting up the application runtime (for example ```WithStaticAppRuntime```, ```WithDynamicAppRuntime```, or ```WithPluginsAppRuntime```), to make sure the runtime accesses the correct locations. 

What is the immediate effect:
* Tenant administrative mode
  * When the application was started without a tenant identifier specified either in command line arguments or in the environment variables. This mode should allow tenant management actions.
  * The folder locations are not changed.
* Tenant specific mode
  * The tenant identifier is passed to the running application.
  * The folder locations (config, licenses, plugins) are found in the ```{tenant}``` subdirectory of the original folder locations.

Check the following packages for more information:
* [Kephas.Services](https://www.nuget.org/packages/Kephas.Services)
* [Kephas.Configuration](https://www.nuget.org/packages/Kephas.Configuration)
* [Kephas.Application.Abstractions](https://www.nuget.org/packages/Kephas.Application.Abstractions)
* [Kephas.Application](https://www.nuget.org/packages/Kephas.Application)

### Example

```c#
public static IAmbientServices SetupAmbientServices(
    this IAmbientServices ambientServices,
    Func<IAmbientServices, IEncryptionService> encryptionServiceFactory,
    IConfiguration? configuration,
    IAppArgs appArgs)
{
    return ambientServices
        .WithDefaultLicensingManager(encryptionServiceFactory(ambientServices))
        // Do not forget setting up the tenant support right *after* setting up the application runtime.
        .WithTenantSupport(appArgs)
        //
        .WithDynamicAppRuntime()
        .WithSerilogManager(configuration);
}
```

## Security

### Tenants management
A global scope ```tenantsmanagement``` permission is provided to require in endpoints or other application actions affecting overall tenants management.

### Tenant admin
A global scope ```tenantadmin``` permission is provided to require in endpoints or other application actions affecting management of the tenant.

### Example

```c#
/// <summary>
/// Endpoint message for adding a tenant.
/// </summary>
[RequiresPermission(typeof(TenantsManagementPermission))]
public class AddTenantMessage : IMessage
{
    //...
}

/// <summary>
/// Endpoint message for changing the tenant information.
/// </summary>
[RequiresPermission(typeof(TenantAdminPermission))]
public class ChangeTenantInfoMessage : IMessage
{
    //...
}
```

## Configuration

### Tenant specific

The ```TenantSettings``` are available through the injectable ```IConfiguration<TenantSettings>``` service. It provides:
* _DisplayName_ (string): The tenant's display name.
* _Domains_ (string[]): an array of domain owned by the tenant. These domains are typically used in the authorization process to identify the tenant a user belongs to.

Example
```c#
var config = injector.Resolve<IConfiguration<TenantSettings>>();
var settings = config.GetSettings(context);
settings.DisplayName = "My changed name";
await config.UpdateSettingsAsync(settings, context).PreserveThreadingContext();
```

### Tenant management
The ```TenantsManagementSettings``` are available through the injectable ```IConfiguration<TenantsManagementSettings>``` service. It provides:
* _GlobalAdmin_ (GlobalAdminSettings): settings for the global tenants administrator.
* _Tenants_ (TenantSettings[]): list of settings for the application tenants.

## IO
By default, the AmbientServices registers the ```FolderLocationsManager``` implementation for the service ```ILocationsManager```.
This normalizes the paths and resolves them relative to the application location. Instead, by using ```IAmbientServices.WithTenantSupport()```,
this global service is replaced with ```TenantFolderLocationsManager``` which appends the tenant identifier prefixed by dot (.) to each of the folders.
This has the following effects:
* The configuration, licenses, plugins, and other locations dependent on the ```ILocationsManager``` service will return different locations for different tenants.
* By prefixing the tenant path element with dot (.), this location is considered hidden by other services and, therefore ignored (checked with ```location.IsHiddenLocation()``` extension method).
