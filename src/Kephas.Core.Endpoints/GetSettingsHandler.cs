// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Configuration;
    using Kephas.Messaging;
    using Kephas.Reflection;

    /// <summary>
    /// A get settings handler.
    /// </summary>
    public class GetSettingsHandler : MessageHandlerBase<GetSettingsMessage, GetSettingsResponseMessage>
    {
        private readonly ICompositionContext compositionContext;
        private readonly ITypeResolver typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSettingsHandler"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        /// <param name="typeResolver">The type resolver.</param>
        public GetSettingsHandler(ICompositionContext compositionContext, ITypeResolver typeResolver)
        {
            this.compositionContext = compositionContext;
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
                };
            }

            await Task.Yield();

            var settingsTypeString = message.SettingsType.ToPascalCase();
            var settingsType = this.typeResolver.ResolveType(settingsTypeString, throwOnNotFound: false);
            if (settingsType == null)
            {
                settingsTypeString = settingsType + "Settings";
                settingsType = this.typeResolver.ResolveType(settingsTypeString, throwOnNotFound: false);
            }

            if (settingsType == null)
            {
                return new GetSettingsResponseMessage
                {
                    Message = $"Settings type {message.SettingsType} not found.",
                };
            }

            var configurationType = typeof(IConfiguration<>).MakeGenericType(settingsType);
            var configuration = this.compositionContext.GetExport(configurationType);
            var settings = configuration.GetPropertyValue(nameof(IConfiguration<CoreSettings>.Settings));
            return new GetSettingsResponseMessage
            {
                Settings = settings,
            };
        }
    }
}