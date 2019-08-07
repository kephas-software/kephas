// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleGameShell.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the role game shell class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RoleGame.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas;
    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Logging.NLog;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    using RoleGame.Composition;
    using RoleGame.Composition.ScopeFactory;
    using RoleGame.Services;

    using CompositionHelper = RoleGame.Composition.CompositionHelper;
    using ScopeNames = RoleGame.Composition.ScopeNames;

    public class RoleGameShell : AppBase
    {
        private IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();

        /// <summary>Bootstraps the application asynchronously.</summary>
        /// <param name="appArgs">The application arguments (optional).</param>
        /// <param name="ambientServices">The ambient services (optional). If not provided then <see cref="P:Kephas.AmbientServices.Instance" /> is considered.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The asynchronous result that yields the <see cref="T:Kephas.Application.IAppContext" />.
        /// </returns>
        public override async Task<IAppContext> BootstrapAsync(
            string[] appArgs = null,
            IAmbientServices ambientServices = null,
            CancellationToken cancellationToken = default)
        {
            var appContext = await base.BootstrapAsync(appArgs, ambientServices, cancellationToken).PreserveThreadContext();
            this.Run(appContext.AmbientServices);

            await appContext.SignalShutdown(appContext);
            return appContext;
        }

        /// <summary>Configures the ambient services asynchronously.</summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override async Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            ambientServicesBuilder
                .WithNLogManager()
                .WithDefaultAppRuntime()
                .WithMefCompositionContainer(
                    b => b.WithScopeFactory<UserMefScopeFactory>()
                          .WithConventions(CompositionHelper.GetConventions(ambientServicesBuilder.AmbientServices.GetService<ITypeLoader>())));
        }

        /// <summary>
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="ambientServices">The configured ambient services.</param>
        protected void Run(IAmbientServices ambientServices)
        {
            var container = ambientServices.CompositionContainer;
            var appManifest = container.GetExport<IAppManifest>();
            Console.WriteLine();
            Console.WriteLine($"Application '{appManifest.AppId} V{appManifest.AppVersion}' started.");

            // TODO...

            var context = CreateUserContext(container, "Adela");
            this.compositionContexts.Add("Adela", context);

            context = CreateUserContext(container, "Ioan");
            this.compositionContexts.Add("Ioan", context);

            var gameManagerUser1 = this.compositionContexts["Ioan"].GetExport<IGameManager>().User.Name;
            var gameManagerUser2 = this.compositionContexts["Adela"].GetExport<IGameManager>().User.Name;

            Console.WriteLine("Press any key to end the program...");
            Console.ReadLine();
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