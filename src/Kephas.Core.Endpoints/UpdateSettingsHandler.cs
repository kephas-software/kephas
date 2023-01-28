﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSettingsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An update settings handler.
    /// </summary>
    public class UpdateSettingsHandler : MessageHandlerBase<UpdateSettingsMessage, Response>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ITypeResolver typeResolver;
        private readonly ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSettingsHandler"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="typeResolver">The type resolver.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="logger">Optional. The logger.</param>
        public UpdateSettingsHandler(
            IServiceProvider serviceProvider,
            ITypeResolver typeResolver,
            ISerializationService serializationService,
            ILogger<UpdateSettingsHandler>? logger = null)
            : base(logger)
        {
            this.serviceProvider = serviceProvider;
            this.typeResolver = typeResolver;
            this.serializationService = serializationService;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override async Task<Response> ProcessAsync(UpdateSettingsMessage message, IMessagingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.SettingsType) && message.Settings is null or string)
            {
                return new Response
                {
                    Message = "Settings type not provided.",
                };
            }

            await Task.Yield();

            Type? settingsType;
            object settings;
            if (message.Settings is string settingsString)
            {
                var settingsTypeString = message.SettingsType!.ToPascalCase();
                settingsType = this.typeResolver.ResolveType(settingsTypeString, throwOnNotFound: false);
                if (settingsType == null)
                {
                    settingsTypeString += "Settings";
                    settingsType = this.typeResolver.ResolveType(settingsTypeString, throwOnNotFound: false);
                }

                if (settingsType == null)
                {
                    return new GetSettingsResponse
                    {
                        Message = $"Settings type {message.SettingsType} not found.",
                    };
                }

                settings = (await this.serializationService.JsonDeserializeAsync(
                    settingsString,
                    ctx => ctx.RootObjectType(settingsType),
                    cancellationToken: token).PreserveThreadContext())!;
            }
            else
            {
                settingsType = message.Settings.GetType();
                settings = message.Settings;
            }

            var configurationType = typeof(IConfiguration<>).MakeGenericType(settingsType);
            var configuration = this.serviceProvider.Resolve(configurationType);
            var updateMethod = (IRuntimeMethodInfo)configuration.GetRuntimeTypeInfo()
                .GetMember(nameof(IConfiguration<NullLogManager>.UpdateSettingsAsync));
            var result = (IOperationResult<bool>)(await updateMethod.InvokeAsync(configuration, new[] { settings, context, token }).PreserveThreadContext());
            return new Response
            {
                Message = result.HasErrors()
                    ? result.Errors().First().Message
                    : result.Value
                        ? "Update successful"
                        : (result.Messages.FirstOrDefault()?.Message ?? "Update did not complete successfully."),
                Severity = result.OperationState == OperationState.Completed
                    ? SeverityLevel.Info
                    : result.OperationState == OperationState.Warning
                        ? SeverityLevel.Warning
                        : SeverityLevel.Info,
            };
        }
    }
}