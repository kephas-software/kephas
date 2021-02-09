// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsAppSetupHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Application
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Configuration;
    using Kephas.Commands;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application setup handler which executes the setup commands.
    /// </summary>
    public class CommandsAppSetupHandler : Loggable, IAppSetupHandler
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<SystemSettings> systemConfiguration;
        private readonly IAppSettingsProvider appSettingsProvider;
        private readonly ICommandProcessor commandProcessor;
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandsAppSetupHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="appSettingsProvider">The provider for <see cref="AppSettings"/>.</param>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public CommandsAppSetupHandler(
            IAppRuntime appRuntime,
            IConfiguration<SystemSettings> systemConfiguration,
            IAppSettingsProvider appSettingsProvider,
            ICommandProcessor commandProcessor,
            IMessageProcessor messageProcessor,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
            this.appSettingsProvider = appSettingsProvider;
            this.commandProcessor = commandProcessor;
            this.messageProcessor = messageProcessor;
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
                .MergeAll(await this.ExecuteRootSetupCommandsAsync(appContext, cancellationToken).PreserveThreadContext())
                .MergeAll(await this.ExecuteInstanceSetupCommandsAsync(appContext, cancellationToken).PreserveThreadContext())
                .Complete();
        }

        private async Task<IOperationResult> ExecuteRootSetupCommandsAsync(IContext appContext, CancellationToken cancellationToken)
        {
            if (!this.appRuntime.IsRoot())
            {
                return new OperationResult().Complete();
            }

            var settings = this.systemConfiguration.GetSettings(appContext);
            if ((settings.SetupCommands?.Length ?? 0) == 0)
            {
                return new OperationResult().Complete();
            }

            // get the commands to execute and clear the list of commands
            // so that, if necessary, they can be added by the executed commands
            var setupCommands = new List<object>(settings.SetupCommands!);
            try
            {
                settings.SetupCommands = null;
                await this.systemConfiguration.UpdateSettingsAsync(context: appContext, cancellationToken: cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(
                    ex,
                    "Error while updating the system settings for '{app}/{appInstance}'",
                    this.appRuntime.GetAppId(),
                    this.appRuntime.GetAppInstanceId());
            }

            // execute the commands
            return await this.ExecuteSetupCommandsAsync(setupCommands, appContext, cancellationToken).PreserveThreadContext();
        }

        private async Task<IOperationResult> ExecuteInstanceSetupCommandsAsync(IContext appContext, CancellationToken cancellationToken)
        {
            var appId = this.appRuntime.GetAppId()!;
            var appSettings = this.appSettingsProvider.GetAppSettings();
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
                await this.systemConfiguration.UpdateSettingsAsync(context: appContext, cancellationToken: cancellationToken)
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
                    var result = await this.ExecuteCommandAsync(cmd, appContext, cancellationToken)
                        .PreserveThreadContext();
                    if (result == null)
                    {
                        this.Logger.Info("Executing the setup command '{command}' did not return any result.", cmd);
                    }
                    else
                    {
                        this.Logger.Info("Executing the setup command '{command}' returned '{result}'", cmd, result);
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

        private async Task<object?> ExecuteCommandAsync(object? cmd, IContext appContext, CancellationToken cancellationToken)
        {
            switch (cmd)
            {
                case null:
                    return null;
                case string cmdLine:
                    var command = Command.Parse(cmdLine);
                    return await this.commandProcessor
                        .ProcessAsync(command.Name, command.Args, appContext, cancellationToken)
                        .PreserveThreadContext();
                default:
                    return await this.messageProcessor
                        .ProcessAsync(cmd, ctx => ctx.Merge(appContext), cancellationToken)
                        .PreserveThreadContext();
            }
        }
    }
}