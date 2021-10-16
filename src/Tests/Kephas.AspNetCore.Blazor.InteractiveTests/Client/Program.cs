// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.Blazor.InteractiveTests.Client
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Extensions.DependencyInjection;
    using Kephas.Extensions.Logging;
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            IAmbientServices ambientServices = new AmbientServices();
            
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.ConfigureContainer(new InjectionServiceProviderFactory(ambientServices));
        
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        
            ambientServices
                .WithServiceCollection(builder.Services)
                .ConfigureExtensionsLogging();
        
            ConfigureAmbientServices(ambientServices);
        
            // TODO this crashes
            var appManager = ambientServices.Injector.Resolve<IAppManager>();
            
            var host = builder.Build();
            await host.RunAsync();
        }

        // public static async Task Main(string[] args)
        // {
        //     var clientApp = new ClientApp<App>(
        //         new AppArgs(args),
        //         ConfigureAmbientServices);
        //
        //     await clientApp.RunAsync();
        // }
        
        private static LoggerConfiguration GetLoggerConfiguration()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.BrowserConsole();
        }

        private static void ConfigureAmbientServices(IAmbientServices ambientServices)
        {
            ambientServices
                .WithSerilogManager(GetLoggerConfiguration())
                .WithStaticAppRuntime()
                .BuildWithAutofac();
        }
    }
}
