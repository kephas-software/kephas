// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore
{
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;

    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Application.Configuration;
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Cryptography.X509Certificates;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Server.Kestrel.Core;

    /// <summary>
    /// Extension methods for <see cref="IWebHostBuilder"/>
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Configures the web host builder using the ambient services.
        /// </summary>
        /// <param name="webHostBuilder">The web host builder.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <returns>The provided web host builder.</returns>
        public static IWebHostBuilder UseAmbientServices(this IWebHostBuilder webHostBuilder, IAmbientServices ambientServices)
        {
            Requires.NotNull(webHostBuilder, nameof(webHostBuilder));
            Requires.NotNull(ambientServices, nameof(ambientServices));

            webHostBuilder
                .UseSetting(WebHostDefaults.ApplicationKey, ambientServices.AppRuntime.GetAppId());

            return webHostBuilder;
        }

        /// <summary>
        /// An IHostBuilder extension method that configures the logger.
        /// </summary>
        /// <param name="hostBuilder">The host builder to act on.</param>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="opts">The Kestrel options.</param>
        /// <returns>
        /// An IWebHostBuilder.
        /// </returns>
        public static IWebHostBuilder UseAppUrls(this IWebHostBuilder hostBuilder, IAmbientServices ambientServices, KestrelServerOptions opts)
        {
            var container = ambientServices.CompositionContainer;
            var appSettingsProvider = container.GetExport<IAppSettingsProvider>();
            var instanceSettings = appSettingsProvider.GetAppSettings();
            var appRuntime = ambientServices.AppRuntime;

            // Workaround for fixing the Autofac/Kestrel issue regarding the DI registration order.
            // https://github.com/aspnet/KestrelHttpServer/issues/1783
            opts.ApplicationServices ??= container.ToServiceProvider();

            var logger = ambientServices.LogManager.GetLogger<StartupAppBase>();
            var urls = instanceSettings?.Host?.Urls;
            if (urls != null)
            {
                var endpointUrls = urls.Select(url => url.Url).ToArray();
                logger.Info(
                    "Using {urls} for {app} in instance {appInstance} (original urls: {originalUrls}).",
                    endpointUrls,
                    appRuntime.GetAppId(),
                    appRuntime.GetAppInstanceId(),
                    urls.Select(url => url.ToString()).ToArray());
                hostBuilder.UseUrls(endpointUrls);
                urls.ForEach(url => hostBuilder.ConfigureUrl(ambientServices, opts, url, logger));
            }
            else
            {
                logger.Info("Using default URLs for {app} in instance {appInstance}.", appRuntime.GetAppId(), appRuntime.GetAppInstanceId());
            }

            return hostBuilder;
        }

        private static IWebHostBuilder ConfigureUrl(this IWebHostBuilder hostBuilder, IAmbientServices ambientServices, KestrelServerOptions opts, UrlSettings urlSettings, ILogger logger)
        {
            var certificate = TryGetCertificate(ambientServices, urlSettings.Certificate);
            logger.Debug(
                "URL: {url}, isAnyIP: {isAnyIP}, isHttps: {isHttps}, cert: {certThumbprint}/{certFriendlyName}",
                urlSettings.Url,
                urlSettings.IsAnyIP,
                urlSettings.IsHttps,
                certificate?.Thumbprint,
                certificate?.FriendlyName);

            if (certificate == null)
            {
                return hostBuilder;
            }

            var uri = urlSettings.GetNormalizedUri()!;
            if (urlSettings.IsAnyIP)
            {
                if (urlSettings.IsHttps)
                {
                    opts.ListenAnyIP(uri.Port, cfg => cfg.UseHttps(certificate));
                }
                else
                {
                    opts.ListenAnyIP(uri.Port);
                }
            }
            else
            {
                var ipAddress = IPAddress.Parse(uri.Host);
                if (urlSettings.IsHttps)
                {
                    opts.Listen(ipAddress, uri.Port, cfg => cfg.UseHttps(certificate));
                }
                else
                {
                    opts.Listen(ipAddress, uri.Port);
                }
            }

            return hostBuilder;
        }

        private static X509Certificate2? TryGetCertificate(IAmbientServices ambientServices, string? certName)
        {
            if (string.IsNullOrEmpty(certName))
            {
                return null;
            }

            var certificateProvider = ambientServices.CompositionContainer.GetExport<ICertificateProvider>();
            return certificateProvider.TryGetCertificate(certName);
        }
    }
}