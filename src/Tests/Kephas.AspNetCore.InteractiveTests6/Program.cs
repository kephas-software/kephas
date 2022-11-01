using Kephas;
using Kephas.Application;
using Kephas.Application.AspNetCore;
using Kephas.AspNetCore.InteractiveTests6.Extensions;
using Kephas.Cryptography;

return await new SwitchApp(args)
    .AddApp((servicesBuilder, appArgs) =>
    {
        return new WebApp(
            appArgs,
            builder =>
            {
                builder.Host
                    .ConfigureHostConfiguration(_ =>
                    {
                        servicesBuilder
                            .WithDefaultLicensingManager(new EncryptionService())
                            .WithDynamicAppRuntime()
                            .AddAppArgs(appArgs);
                    })
                    .ConfigureServices((ctx, services) =>
                    {
                        servicesBuilder.WithSerilogManager(ctx.Configuration);
                        services.UseServicesBuilder(servicesBuilder);
                    });
            });
    }).RunAsync(1);

class EncryptionService : AesEncryptionService
{
}