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
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;

    /// <summary>
    /// Handler for <see cref="GetSettingsTypesMessage"/>.
    /// </summary>
    public class GetSettingsTypesHandler : IMessageHandler<GetSettingsTypesMessage, GetSettingsTypesResponse>
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
        public async Task<GetSettingsTypesResponse> ProcessAsync(GetSettingsTypesMessage message, IMessagingContext context, CancellationToken token)
        {
            await Task.Yield();

            var settingsTypes = this.appRuntime.GetAppAssemblies()
                .SelectMany(a => this.typeLoader.GetExportedTypes(a)
                        .Where(t => typeof(ISettings).IsAssignableFrom(t) && !t.IsAbstract))
                .ToList();

            var dynTypeRegistry = new DynamicTypeRegistry();
            settingsTypes.ForEach(t => dynTypeRegistry.Types.Add(GetDynamicTypeInfo(t)));
            return new GetSettingsTypesResponse
            {
                SettingsTypes = dynTypeRegistry.Types.ToArray(),
            };
        }

        private static DynamicTypeInfo GetDynamicTypeInfo(Type t)
        {
            var dti = new DynamicTypeInfo
            {
                Name = t.Name,
                Namespace = t.Namespace,
                FullName = t.FullName ?? t.Name,
            };

            dti.Annotations.AddRange(t.GetCustomAttributes(inherit: true)
                .OfType<IMetadataProvider>()
                .Select(a =>
                {
                    var e = new Expando();
                    a.GetMetadata().ForEach(m => e[m.name] = m.value);
                    return (object)e;
                }));
            return dti;
        }
    }
}