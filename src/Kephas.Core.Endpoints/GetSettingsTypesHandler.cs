// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetSettingsTypesHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// Handler for <see cref="GetSettingsTypesMessage"/>.
    /// </summary>
    public class GetSettingsTypesHandler : MessageHandlerBase<GetSettingsTypesMessage, GetSettingsTypesResponseMessage>
    {
        private readonly IAppRuntime appRuntime;
        private readonly ITypeLoader typeLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetSettingsTypesHandler"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public GetSettingsTypesHandler(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>
        /// The response promise.
        /// </returns>
        public override async Task<GetSettingsTypesResponseMessage> ProcessAsync(GetSettingsTypesMessage message, IMessagingContext context, CancellationToken token)
        {
            await Task.Yield();

            var settingsTypes = this.appRuntime.GetAppAssemblies()
                .SelectMany(a => this.typeLoader.GetExportedTypes(a)
                        .Where(t => typeof(ISettings).IsAssignableFrom(t) && !t.IsAbstract))
                .ToList();

            return new GetSettingsTypesResponseMessage
            {
                SettingsTypes = settingsTypes
                    .Select(t => (ITypeInfo)new DynamicTypeInfo { Name = t.Name, Namespace = t.Namespace, FullName = t.FullName ?? t.Name })
                    .ToArray(),
            };
        }
    }
}