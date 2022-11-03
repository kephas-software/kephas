using Kephas;
using Kephas.Application;
using Kephas.Application.AspNetCore;
using Kephas.AspNetCore.InteractiveTests6.Extensions;
using Kephas.Cryptography;
using Kephas.Extensions.DependencyInjection;

return await new SwitchApp(args)
    .AddApp((servicesBuilder, appArgs) =>
    {
        return new WebApp(
            appArgs,
            builder =>
            {
                builder.Host
                    .ConfigureServices((ctx, services) =>
                    {
                        servicesBuilder
                            .WithDefaultLicensingManager(new AesEncryptionService())
                            .WithDynamicAppRuntime()
                            .AddAppArgs(appArgs)
                            .WithSerilogManager(ctx.Configuration);
                    });
            });
    }).RunAsync(1);
