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
    public class RoleGameShutdownAwaiter : IAppShutdownAwaiter
    {
        private readonly ICompositionContext compositionContext;
        private readonly IConsole console;
        private readonly IDictionary<string, ICompositionContext> compositionContexts = new Dictionary<string, ICompositionContext>();


        /// <summary>
        /// Initializes a new instance of the RoleGame.Application.RoleGameShutdownAwaiter class.
        /// </summary>
        /// <param name="console">The console.</param>
        public RoleGameShutdownAwaiter(ICompositionContext compositionContext, IConsole console)
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
        public async Task<(IOperationResult result, AppShutdownInstruction instruction)> WaitForShutdownSignalAsync(CancellationToken cancellationToken = default)
        {
            this.console.WriteLine(string.Empty);
            this.console.WriteLine($"Application started.");

            var context = CreateUserContext("Adela");
            this.compositionContexts.Add("Adela", context);

            context = CreateUserContext("Ioan");
            this.compositionContexts.Add("Ioan", context);

            var gameManagerUser1 = this.compositionContexts["Ioan"].GetExport<IGameManager>().User.Name;
            var gameManagerUser2 = this.compositionContexts["Adela"].GetExport<IGameManager>().User.Name;

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
