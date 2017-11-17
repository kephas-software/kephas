// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoleGameShell.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Diagnostics;
    using Kephas.Logging.NLog;
    using Kephas.Platform.Net;
    using Kephas.Reflection;

    using RoleGame.Composition;
    using RoleGame.Composition.ScopeFactory;
    using RoleGame.Services;

    using AppContext = Kephas.Application.AppContext;
    using ScopeNames = RoleGame.Composition.ScopeNames;

    public class RoleGameShell : AppBase
    {
        private IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();

        /// <summary>Configures the ambient services asynchronously.</summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServicesBuilder">The ambient services builder.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override Task ConfigureAmbientServicesAsync(
            string[] appArgs,
            AmbientServicesBuilder ambientServicesBuilder,
            CancellationToken cancellationToken)
        {
            return ambientServicesBuilder
                .WithNLogManager()
                .WithNetAppRuntime()
                .WithMefCompositionContainerAsync(b => b.WithScopeFactory<UserMefScopeFactory>()
                    .WithConventions(CompositionHelper.GetConventions(ambientServicesBuilder.AmbientServices.GetService<ITypeLoader>())));
        }

        /// <summary>
        /// Executes the application main functionality asynchronously.
        /// </summary>
        /// <remarks>
        /// This method should be overwritten to provide a meaningful content.
        /// </remarks>
        /// <param name="appArgs">The application arguments.</param>
        /// <param name="ambientServices">The configured ambient services.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected override Task RunAsync(string[] appArgs, IAmbientServices ambientServices, CancellationToken cancellationToken)
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

            return base.RunAsync(appArgs, ambientServices, cancellationToken);
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