namespace SimpleModel.App.Console.Application
{
    using System;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Model;

    public class Shell
    {
        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        public async Task StartAppAsync()
        {
            this.NotifyAppStarting();

            var ambientServices = await AmbientServicesInitializer.InitializeAsync();

            this.NotifyAppStarted(ambientServices);

            var modelSpaceProvider = ambientServices.CompositionContainer.GetExport<IModelSpaceProvider>();
            var modelSpace = modelSpaceProvider.GetModelSpace();
        }

        private void NotifyAppStarting()
        {
            Console.WriteLine("Application initializing...");
        }

        private void NotifyAppStarted(AmbientServices ambientServices)
        {
            var elapsed = (TimeSpan)((dynamic)ambientServices).Elapsed;
            var appManifest = ambientServices.CompositionContainer.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");
        }
    }
}