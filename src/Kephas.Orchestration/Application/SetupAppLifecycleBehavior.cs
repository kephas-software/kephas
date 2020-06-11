// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupAppLifecycleBehavior.cs" company="Kephas Software SRL">
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
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Application lifecycle behavior for setting up the application.
    /// </summary>
    [ProcessingPriority(Priority.Highest)]
    public class SetupAppLifecycleBehavior : AppLifecycleBehaviorBase
    {
        private readonly IAppRuntime appRuntime;
        private readonly IConfiguration<SystemSettings> systemConfiguration;
        private readonly ICommandProcessor commandProcessor;
        private readonly IMessageProcessor messageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupAppLifecycleBehavior"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="systemConfiguration">The system configuration.</param>
        /// <param name="commandProcessor">The command processor.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        public SetupAppLifecycleBehavior(
            IAppRuntime appRuntime,
            IConfiguration<SystemSettings> systemConfiguration,
            ICommandProcessor commandProcessor,
            IMessageProcessor messageProcessor,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.appRuntime = appRuntime;
            this.systemConfiguration = systemConfiguration;
            this.commandProcessor = commandProcessor;
            this.messageProcessor = messageProcessor;
        }

        /// <summary>
        /// Interceptor called after the application completes its asynchronous initialization.
        /// </summary>
        /// <param name="appContext">Context for the application.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public override async Task AfterAppInitializeAsync(IAppContext appContext, CancellationToken cancellationToken = default)
        {
            var appId = this.appRuntime.GetAppId()!;
            var systemSettings = this.systemConfiguration.Settings;
            if (systemSettings.Instances == null)
            {
                return;
            }

            if (!systemSettings.Instances.TryGetValue(appId, out var appSettings) || appSettings.SetupCommands == null)
            {
                return;
            }

            var setupCommands = new List<object>(appSettings.SetupCommands);
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
                    this.Logger.Error(ex, "Error while executing the setup command '{command}'", cmd);
                }
            }

            try
            {
                appSettings.SetupCommands = null;
                await this.systemConfiguration.UpdateSettingsAsync(cancellationToken: cancellationToken)
                    .PreserveThreadContext();
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex, "Error while updating the system settings for '{app}/{appInstance}'", appId, this.appRuntime.GetAppInstanceId());
            }
        }

        private async Task<object?> ExecuteCommandAsync(object? cmd, IAppContext appContext, CancellationToken cancellationToken)
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