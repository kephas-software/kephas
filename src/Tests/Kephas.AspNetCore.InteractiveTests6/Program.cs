using Kephas;
using Kephas.Application;
using Kephas.Application.AspNetCore;
using Kephas.AspNetCore;
using Kephas.AspNetCore.InteractiveTests6.Extensions;
using Kephas.Cryptography;
using Kephas.Extensions.Hosting;

return await new SwitchApp(args)
    .AddApp((ambientServices, appArgs) =>
    {
        return new WebApp(
            appArgs,
            builder =>
            {
                builder.Host
                    .ConfigureAmbientServices(
                        ambientServices,
                        appArgs,
                        ambient => ambient.WithDynamicAppRuntime().BuildWithAutofac(),
                        (services, ambient) =>
                            ambient.SetupAmbientServices(
                                CreateEncryptionService,
                                services.TryGetStartupService<IConfiguration>()));
            });
    }).RunAsync(1);

static IEncryptionService CreateEncryptionService(IAmbientServices ambientServices)
{
return new EncryptionService(() => new EncryptionContext(ambientServices.Injector));
}

class EncryptionService : AesEncryptionService
{
    public EncryptionService(Func<IEncryptionContext> contextCtor)
        : base(contextCtor)
    {
    }
}