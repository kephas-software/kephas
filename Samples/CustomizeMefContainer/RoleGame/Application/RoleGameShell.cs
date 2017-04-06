using Kephas.Reflection;

namespace RoleGame.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Diagnostics;
    using Kephas.Platform.Net;
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
                            .WithNetAppRuntime()
                            .WithMefCompositionContainerAsync(b => b.WithScopeFactory<UserMefScopeFactory>()
                                                                    .WithConventions(CompositionHelper.GetConventions(ambientServicesBuilder.AmbientServices.GetService<ITypeLoader>())));

                    var compositionContainer = ambientServicesBuilder.AmbientServices.CompositionContainer;
                    var appManager = compositionContainer.GetExport<IAppManager>();
                    await appManager.InitializeAppAsync(new AppContext());
                });

            var container = ambientServicesBuilder.AmbientServices.CompositionContainer;
            var appManifest = container.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started. Elapsed: {elapsed:c}.");

            // TODO...

            var context = CreateUserContext(container, "Adela");
            this.compositionContexts.Add("Adela", context);

            context = CreateUserContext(container, "Ioan");
            this.compositionContexts.Add("Ioan", context);

            var gameManagerUser1 = this.compositionContexts["Ioan"].GetExport<IGameManager>().User.Name;
            var gameManagerUser2 = this.compositionContexts["Adela"].GetExport<IGameManager>().User.Name;
        }

        private static ICompositionContext CreateUserContext(ICompositionContext container, string name)
        {
            var context = container.CreateScopedContext(ScopeNames.User);
            var user = context.GetExport<IUser>();
            user.Name = name;
            return context;
        }
    }
}