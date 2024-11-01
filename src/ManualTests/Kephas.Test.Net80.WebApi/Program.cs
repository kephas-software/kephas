using System.IO.Compression;
using Kephas.Extensions;
using Kephas.Web.Hosting;
using Kephas.Web.Mvc;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWebLogging();

builder.WebHost
    .UseContentRoot(Directory.GetCurrentDirectory())
    .UseKestrel();

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureMiddleware(app);

await app.RunAsync();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add the web api infrastructure
    services.AddApplicationServices(configuration);

    services.AddHttpContextAccessor();

    // Add compression and anti-forgery.
    services.AddResponseCompression(
        options =>
        {
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
        });

    services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });

    services.AddAntiforgery();

    services.ConfigureControllers();
}

void ConfigureMiddleware(WebApplication app)
{
    // Configure the HTTP request pipeline.

    app.UseHttpsRedirection();

    app.UseStaticFiles();
    app.UseResponseCompression();

    app.MapControllers();
}
