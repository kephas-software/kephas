using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    using System.Diagnostics;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition.Mef;
    using Kephas.Hosting.Net45;
    using Kephas.Logging.NLog;

    class Program
    {
        static void Main(string[] args)
        {
            StartAppAsync();

            Console.ReadLine();
        }

        static async Task StartAppAsync()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.WriteLine("Application starting...");

            var ambientServicesBuilder = await new AmbientServicesBuilder()
                    .WithNLogManager()
                    .WithNet45HostingEnvironment()
                    .WithMefCompositionContainerAsync();

            var bootstrapper = ambientServicesBuilder.AmbientServices.CompositionContainer.GetExport<IAppBootstrapper>();
            await bootstrapper.StartAsync();

            stopwatch.Stop();
            Console.WriteLine($"Application started. Elapsed: {stopwatch.Elapsed:c}.");
        }
    }
}
