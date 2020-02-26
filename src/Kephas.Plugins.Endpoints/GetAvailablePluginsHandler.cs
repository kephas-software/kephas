// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetAvailablePluginsHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the get available plugins message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Endpoints
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Logging;
    using Kephas.Messaging;
    using Kephas.Plugins;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A get available plugins handler.
    /// </summary>
    public class GetAvailablePluginsHandler : MessageHandlerBase<GetAvailablePluginsMessage, GetAvailablePluginsResponseMessage>
    {
        private readonly IPluginManager pluginManager;
        private readonly IAppContext appContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailablePluginsHandler"/> class.
        /// </summary>
        /// <param name="pluginManager">Manager for plugins.</param>
        /// <param name="appContext">Context for the application.</param>
        public GetAvailablePluginsHandler(IPluginManager pluginManager, IAppContext appContext)
        {
            this.pluginManager = pluginManager;
            this.appContext = appContext;
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
        public override async Task<GetAvailablePluginsResponseMessage> ProcessAsync(GetAvailablePluginsMessage message, IMessagingContext context, CancellationToken token)
        {
            this.appContext.Logger.Info("Retrieving {count} packages for {search}...", message.Take, message.SearchTerm ?? "<all>");

            var result = await this.pluginManager.GetAvailablePluginsAsync(
                f => f.SearchTerm(message.SearchTerm)
                      .IncludePrerelease(message.IncludePrerelease)
                      .Skip(message.Skip)
                      .Take(message.Take),
                cancellationToken: token).PreserveThreadContext();
            var plugins = result.ReturnValue;

            var searchTerm = message.SearchTerm ?? "<all>";
            this.appContext.Logger.Info("Retrieved {count} packages for {search}. Elapsed: {elapsed:c}.", plugins.Count(), searchTerm, result.Elapsed);

            return new GetAvailablePluginsResponseMessage
                {
                    Message = $"Retrieved {plugins.Count()} packages for {searchTerm}. Elapsed: {result.Elapsed:c}.",
                    Plugins = plugins.ToDictionary(p => p.Identity.Id, p => p.Identity.Version),
                };
        }
    }
}
