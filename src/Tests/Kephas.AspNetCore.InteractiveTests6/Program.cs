using Kephas;
using Kephas.Application;
using Kephas.AspNetCore;
using Kephas.AspNetCore.InteractiveTests6.Extensions;
using Kephas.Cryptography;
using Kephas.Extensions.Hosting;

var ambientServices = new AmbientServices();
await new WebApp(
        new AppArgs(args),
        builder =>
        {
            builder.Host
                .ConfigureAmbientServices(
                    ambientServices,
                    args,
                    ambient => ambient.BuildWithAutofac(),
                    (services, ambient) =>
                        ambient.SetupAmbientServices(
                            CreateEncryptionService,
                            services.TryGetStartupService<IConfiguration>()));
        })
    .RunAsync();

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