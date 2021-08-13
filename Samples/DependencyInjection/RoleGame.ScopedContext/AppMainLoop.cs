// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppMainLoop.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Composition;
using Kephas.Operations;
using RoleGame.Services;

namespace RoleGame.ScopedContext
{
    public class AppMainLoop : IAppMainLoop
    {
        private readonly ICompositionContext compositionContext;
        private readonly IConsole console;
        private readonly IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();


        /// <summary>
        /// Initializes a new instance of the RoleGame.Application.RoleGameShutdownAwaiter class.
        /// </summary>
        /// <param name="console">The console.</param>
        public AppMainLoop(ICompositionContext compositionContext, IConsole console)
        {
            this.compositionContext = compositionContext;
            this.console = console;
        }

        /// <summary>
        /// Waits for the shutdown signal asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Optional. A token that allows processing to be
        ///                                 cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the shutdown result.
        /// </returns>
        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> Main(CancellationToken cancellationToken = default)
        {
            console.WriteLine(string.Empty);
            console.WriteLine($"Application started.");

            var context = CreateUserContext("Ciuri");
            compositionContexts.Add("Ciuri", context);

            context = CreateUserContext("Buri");
            compositionContexts.Add("Buri", context);

            var gameManagerUser1 = compositionContexts["Ciuri"].GetExport<IGameManager>().User.Name;
            var gameManagerUser2 = compositionContexts["Buri"].GetExport<IGameManager>().User.Name;

            console.WriteLine($"User in composition context Ciuri: {gameManagerUser1}.");
            console.WriteLine($"User in composition context Buri: {gameManagerUser2}.");

            console.WriteLine("Press any key to end the program...");
            console.ReadLine();

            return (null, AppShutdownInstruction.Shutdown);
        }

        private ICompositionContext CreateUserContext(string name)
        {
            var context = compositionContext.CreateScopedContext();
            var user = context.GetExport<IUser>();
            user.Name = name;
            return context;
        }
    }
}
