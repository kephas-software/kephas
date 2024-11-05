using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSwag;
using NSwag.AspNetCore;

namespace Kephas.Web.OpenApi;

/// <summary>
/// Provides extension methods to the NSwag (OpenAPI) package.
/// </summary>
[ExcludeFromCodeCoverage]
public static class OpenApiExtensions
{
    /// <summary>
    /// The default section for Open API settings.
    /// </summary>
    public static readonly string OpenApiSection = "OpenApi";

    /// <summary>
    /// Configures the <see cref="IServiceCollection" /> to add the Open API documents.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="config">The application configuration.</param>
    /// <returns>The instance of the <see cref="IServiceCollection" /> allowing to fluently chain the methods.</returns>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, IConfiguration config)
        => AddOpenApiDocumentation(services, options => config.GetSection(OpenApiSection).Bind(options));

    /// <summary>
    /// Configures the <see cref="IServiceCollection" /> to add the Open API documents.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="config">The Open API options configurator.</param>
    /// <returns>The instance of the <see cref="IServiceCollection" /> allowing to fluently chain the methods.</returns>
    public static IServiceCollection AddOpenApiDocumentation(this IServiceCollection services, Action<OpenApiSettings> config)
    {
        services.Configure(config ?? (_ => { }));

        var options = new OpenApiSettings();
        config?.Invoke(options);
        if (!options.Enabled)
        {
            return services;
        }
            
        // Register API for all versions
        var versions = options.Versions
            .Distinct()
            .OrderBy(v => v)
            .ToList();
        foreach (var version in versions)
        {
            // Register the Swagger services
            services.AddOpenApiDocument(
                settings =>
                {
                    settings.DocumentName = $"V{version}";
                    settings.ApiGroupNames = new[] { version.ToString() };

                    settings.Description = options.Description;
                    settings.Title = options.Title;
                        
                    settings.PostProcess = document =>
                    {
                        document.Consumes = new List<string> { "application/json", "application/xml" };
                        document.Produces = new List<string> { "application/json", "application/xml" };
                    };

                    settings.AddSecurity(
                        "Bearer",
                        new List<string>(),
                        new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                            Name = "Authorization",
                            In = OpenApiSecurityApiKeyLocation.Header,
                            Type = OpenApiSecuritySchemeType.ApiKey
                        });
                });
        }

        return services;
    }

    /// <summary>
    /// Configures the <see cref="IApplicationBuilder" /> to use Open API documents and the Swagger UI.
    /// </summary>
    /// <param name="app">The instance of the <see cref="IApplicationBuilder" />.</param>
    /// <param name="openApiConfig">The Oen API configuration. Optional.</param>
    /// <param name="swaggerConfig">The swagger configuration. Optional.</param>
    /// <returns>The instance of the <see cref="IApplicationBuilder"/> allowing to fluently chain the methods.</returns>
    public static IApplicationBuilder UseOpenApiDocumentation(this IApplicationBuilder app, Action<OpenApiDocumentMiddlewareSettings>? openApiConfig = null, Action<SwaggerUiSettings>? swaggerConfig = null)
    {
        var optionsGetter = app.ApplicationServices.GetService<IOptions<OpenApiSettings>>();
        var options = optionsGetter?.Value ?? new();

        if (!options.Enabled)
        {
            return app;
        }
            
        // Use OpenAPI document generator.
        app.UseOpenApi(openApiConfig);

        // And the Swagger UI.
        app.UseSwaggerUi(swaggerConfig);

        return app;
    }
}