// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsAppSetupHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Application.Interaction;
    using Kephas.Commands;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application setup handler which executes the setup commands.
    /// </summary>
    public class CommandsAppSetupHandler : Loggable, IAppSetupHandler
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<AppSettings> appConfiguration;
        private readonly IEventHub eventHub;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsAppSetupHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="appConfiguration">The provider for <see cref="AppSettings"/>.</param>
        /// <param name="eventHub">The event hub.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public CommandsAppSetupHandler(
            IAppRuntime appRuntime,
            IConfiguration<AppSettings> appConfiguration,
            IEventHub eventHub,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.appConfiguration = appConfiguration;
            this.eventHub = eventHub;
        }

        /// <summary>
        /// Performs one step in the application setup.
        /// </summary>
        /// <param name="appContext">The application context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public virtual async Task<IOperationResult> SetupAsync(IContext appContext, CancellationToken cancellationToken = default)
        {
            return new OperationResult()
                .MergeAll(await this.ExecuteSetupCommandsAsync(appContext, cancellationToken).PreserveThreadContext())
                .Complete();
        }

        /// <summary>
        /// Executes a setup command.
        /// </summary>
        /// <param name="cmd">The command object.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The asynchronous result.</returns>
        protected virtual Task<IOperationResult<IEnumerable<object?>>> ExecuteCommandAsync(object cmd, IContext context, CancellationToken cancellationToken)
        {
            return this.eventHub.PublishAsync(new ExecuteStartupCommandSignal(cmd, context), context, cancellationToken);
        }

        private async Task<IOperationResult> ExecuteSetupCommandsAsync(IContext appContext, CancellationToken cancellationToken)
        {
            var appId = this.appRuntime.GetAppId()!;
            var appSettings = this.appConfiguration.GetSettings(appContext);
            if (appSettings?.SetupCommands == null)
            {
                return new OperationResult().Complete();
            }

            // get the commands to execute and clear the list of commands
            // so that, if necessary, they can be added by the executed commands
            var setupCommands = new List<object>(appSettings.SetupCommands);
            try
            {
                appSettings.SetupCommands = null;
                await this.appConfiguration.UpdateSettingsAsync(context: appContext, cancellationToken: cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(
                    ex,
                    "Error while updating the system settings for '{app}/{appInstance}'",
                    appId,
                    this.appRuntime.GetAppInstanceId());
            }

            // execute the commands
            return await this.ExecuteSetupCommandsAsync(setupCommands, appContext, cancellationToken).PreserveThreadContext();
        }

        private async Task<IOperationResult> ExecuteSetupCommandsAsync(IEnumerable<object> setupCommands, IContext appContext, CancellationToken cancellationToken)
        {
            var opResult = new OperationResult();
            foreach (var cmd in setupCommands)
            {
                try
                {
                    if (cmd == null || (cmd is string cmdString && string.IsNullOrWhiteSpace(cmdString)))
                    {
                        this.Logger.Info("Encountered an empty command in the list of setup commands.");
                    }
                    else
                    {
                        var cmdResult = await this.ExecuteCommandAsync(cmd, appContext, cancellationToken).PreserveThreadContext();
                        var result = cmdResult.Value.LastOrDefault(r => r != null);
                        if (result == null)
                        {
                            this.Logger.Info("Executing the setup command '{command}' did not return any result.", cmd);
                        }
                        else
                        {
                            this.Logger.Info("Executing the setup command '{command}' returned '{result}'", cmd, result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    opResult.MergeException(new AggregateException($"Error while executing the setup command '{cmd}'", ex));
                    this.Logger.Error(ex, "Error while executing the setup command '{command}'", cmd);
                }
            }

            return opResult.Complete();
        }
    }
}