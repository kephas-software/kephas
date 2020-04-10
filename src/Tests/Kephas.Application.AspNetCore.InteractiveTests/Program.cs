namespace Kephas.Application.AspNetCore.InteractiveTests
{
    using Kephas.Extensions.DependencyInjection;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new CompositionServiceProviderFactory(new AmbientServices()))
                    .ConfigureWebHostDefaults(
                        webBuilder => webBuilder
                                        .UseUrls("http://*:5100", "https://*:5101")
                                        .UseStartup<Startup>())
                    .ConfigureAppConfiguration(b => b.AddJsonFile("appSettings.json"));
        }
    }
}
