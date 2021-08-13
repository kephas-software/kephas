using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kephas.Application;
using Kephas.Application.Console;
using Kephas.Composition;
using Kephas.Operations;
using RoleGame.Services;

namespace RoleGame.Application
{
    public class RoleGameMainLoop : IAppMainLoop
    {
        private readonly ICompositionContext compositionContext;
        private readonly IConsole console;
        private readonly IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();


        /// <summary>
        /// Initializes a new instance of the RoleGame.Application.RoleGameShutdownAwaiter class.
        /// </summary>
        /// <param name="console">The console.</param>
        public RoleGameMainLoop(ICompositionContext compositionContext, IConsole console)
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
            this.console.WriteLine(string.Empty);
            this.console.WriteLine($"Application started.");

            var context = CreateUserContext("Ciuri");
            this.compositionContexts.Add("Ciuri", context);

            context = CreateUserContext("Buri");
            this.compositionContexts.Add("Buri", context);

            var gameManagerUser1 = this.compositionContexts["Ciuri"].GetExport<IGameManager>().User.Name;
            var gameManagerUser2 = this.compositionContexts["Buri"].GetExport<IGameManager>().User.Name;

            console.WriteLine($"User in composition context Ciuri: {gameManagerUser1}.");
            console.WriteLine($"User in composition context Buri: {gameManagerUser2}.");

            console.WriteLine("Press any key to end the program...");
            console.ReadLine();

            return (null, AppShutdownInstruction.Shutdown);
        }

        private ICompositionContext CreateUserContext(string name)
        {
            var context = this.compositionContext.CreateScopedContext();
            var user = context.GetExport<IUser>();
            user.Name = name;
            return context;
        }
    }
}
