// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Kephas.Configuration;
    using Kephas.ExceptionHandling;
    using Kephas.Messaging;
    using Kephas.Reflection;

    /// <summary>
    /// A get settings handler.
    /// </summary>
    public class GetSettingsHandler : MessageHandlerBase<GetSettingsMessage, GetSettingsResponseMessage>
    {
        private const string SettingsEnding = "Settings";

        private readonly IInjector injector;
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSettingsHandler"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public GetSettingsHandler(IInjector injector, ITypeResolver typeResolver)
        {
            this.injector = injector;
            this.typeResolver = typeResolver;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override async Task<GetSettingsResponseMessage> ProcessAsync(GetSettingsMessage message, IMessagingContext context, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message.SettingsType))
            {
                return new GetSettingsResponseMessage
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
                return new GetSettingsResponseMessage
                {
                    Message = $"Settings type {message.SettingsType} not found.",
                    Severity = SeverityLevel.Error,
                };
            }

            var configurationType = typeof(IConfiguration<>).MakeGenericType(settingsType);
            var configuration = this.injector.Resolve(configurationType);
            var getSettings = configurationType.GetMethod(nameof(IConfiguration<CoreSettings>.GetSettings));
            var settings = getSettings?.Call(configuration, context);
            return new GetSettingsResponseMessage
            {
                Settings = settings,
            };
        }
    }
}