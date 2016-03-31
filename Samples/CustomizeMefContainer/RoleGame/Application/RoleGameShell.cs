namespace RoleGame.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Hosting.Net45;
    using Kephas.Logging.NLog;

    using RoleGame.Composition;
    using RoleGame.Composition.ScopeFactory;
    using RoleGame.Services;

    using AppContext = Kephas.Application.AppContext;
    using ScopeNames = RoleGame.Composition.ScopeNames;

    public class RoleGameShell
    {
        private IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();

        /// <summary>
        /// Starts the application asynchronously.
        /// </summary>
        /// <returns>
        /// A task.
        /// </returns>
        public async Task StartAppAsync()
        {
            Console.WriteLine("Application initializing...");

            var ambientServicesBuilder = new AmbientServicesBuilder();
            var elapsed = await Profiler.WithStopwatchAsync(
                async () =>
                {
                    await ambientServicesBuilder
                            .WithNLogManager()
                            .WithNet45HostingEnvironment()
                            .WithMefCompositionContainerAsync(b => b.WithScopeFactory<UserMefScopeFactory>()
                                                                    .WithConventions(CompositionHelper.GetConventions()));

                    var compositionContainer = ambientServicesBuilder.AmbientServices.CompositionContainer;
                    var appBootstrapper = compositionContainer.GetExport<IAppBootstrapper>();
                    await appBootstrapper.StartAsync(new AppContext());
                });

            var container = ambientServicesBuilder.AmbientServices.CompositionContainer;
            var appManifest = container.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            Console.WriteLine();
            Console.WriteLine("Provide an operation in form of: term1 op term2. End the program with q instead of an operation.");

            // TODO...

            var context = CreateContext(container, "Adela");
            this.compositionContexts.Add("Adela", context);

            context = CreateContext(container, "Ioan");
            this.compositionContexts.Add("Ioan", context);

            var assertManagerUser1 = this.compositionContexts["Ioan"].GetExport<IGameManager>().User.Name;
            var assertManagerUser2 = this.compositionContexts["Adela"].GetExport<IGameManager>().User.Name;
        }

        private static ICompositionContext CreateContext(ICompositionContext container, string name)
        {
            var context = container.CreateScopedContext(ScopeNames.User);
            var user = context.GetExport<IUser>();
            user.Name = name;
            return context;
        }
    }
}