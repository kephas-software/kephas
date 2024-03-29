﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Reflection;

    /// <summary>
    /// A get settings handler.
    /// </summary>
    public class GetSettingsHandler : IMessageHandler<GetSettingsMessage, GetSettingsResponse>
    {
        private const string SettingsEnding = "Settings";

        private readonly IServiceProvider serviceProvider;
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSettingsHandler"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public GetSettingsHandler(IServiceProvider serviceProvider, ITypeResolver typeResolver)
        {
            this.serviceProvider = serviceProvider;
            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public async Task<GetSettingsResponse> ProcessAsync(GetSettingsMessage message, IMessagingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.SettingsType))
            {
                return new GetSettingsResponse
                {
                    Message = "Settings type not provided.",
                    Severity = SeverityLevel.Error,
                };
            }

            await Task.Yield();

            var settingsTypeString = message.SettingsType!.ToPascalCase();
            if (!settingsTypeString.EndsWith(SettingsEnding, StringComparison.OrdinalIgnoreCase))
            {
                settingsTypeString += SettingsEnding;
            }

            var settingsType = this.typeResolver.ResolveType(settingsTypeString, throwOnNotFound: false);
            if (settingsType == null)
            {
                return new GetSettingsResponse
                {
                    Message = $"Settings type {message.SettingsType} not found.",
                    Severity = SeverityLevel.Error,
                };
            }

            var configurationType = typeof(IConfiguration<>).MakeGenericType(settingsType);
            var configuration = this.serviceProvider.Resolve(configurationType);
            var getSettings = configurationType.GetMethod(nameof(IConfiguration<NullLogManager>.GetSettings));
            var settings = getSettings?.Call(configuration, context);
            return new GetSettingsResponse
            {
                Settings = settings,
            };
        }
    }
}