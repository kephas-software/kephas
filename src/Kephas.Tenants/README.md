# Tenants

## Introduction
This package adds support for multi-tenant applications.

How it works:
* Include the ```Kephas.Tenants``` package in the application deployment.
* The application receives the tenant identifier through the ```tenant``` command line argument or the ```KEPHASAPP_Tenant``` environment variable.
* The tenant is available in code through ```IAppArgs.Tenant()``` extension method or the ```IContext.Tenant()``` extension method.
  * Note that ```IAppArgs``` are available through dependency injection and ```IContext``` instances through the parameter received in context dependent invocations. 
* When setting up the ```IAmbientServices``` during application bootstrapping, make sure to invoke the ```ambientServices.WithTenantSupoport()```.
  * Do this only after setting up the application runtime (for example ```WithStaticAppRuntime```, ```WithDynamicAppRuntime```, or ```WithPluginsAppRuntime```). 

What is the immediate effect:
* Tenant administrative mode
  * When the application was started without a tenant identifier specified either in command line arguments or in the environment variables. This mode should allow tenant management actions.
  * The folder locations are not changed.
* Tenant specific mode
  * The tenant identifier is passed to the running application.
  * The folder locations (config, licenses, plugins) are found in the ```{tenant}``` subdirectory of the original folder locations.

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
        .WithDynamicAppRuntime()
        // Do not forget setting up the tenant support right *after* setting up the application runtime.
        .WithTenantSupport(appArgs)
        //
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
