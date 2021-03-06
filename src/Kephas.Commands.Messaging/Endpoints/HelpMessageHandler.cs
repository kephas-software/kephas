// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the help message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Commands.Messaging.Resources;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// A help message handler.
    /// </summary>
    public class HelpMessageHandler : MessageHandlerBase<HelpMessage, HelpResponseMessage>
    {
        private readonly ICollection<ICommandRegistry> registries;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpMessageHandler"/> class.
        /// </summary>
        /// <param name="registries">The command registries.</param>
        public HelpMessageHandler(ICollection<Lazy<ICommandRegistry, AppServiceMetadata>> registries)
        {
            this.registries = registries.Order()
                .Select(r => r.Value)
                .ToList();
        }

        /// <summary>
        /// Processes the provided message asynchronously and returns a response promise.
        /// </summary>
        /// <param name="message">The message to be handled.</param>
        /// <param name="context">The processing context.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The response promise.</returns>
        public override Task<HelpResponseMessage> ProcessAsync(HelpMessage message, IMessagingContext context, CancellationToken token)
        {
            var matchingCommands = this.registries
                .SelectMany(r => r.GetCommandTypes(message.Command))
                .OrderBy(c => c.Name)
                .ToList();

            var commands = matchingCommands.ToDictionary(
                t => t.Name,
                t => t);

            var response = new HelpResponseMessage();

            if (commands.Count > 1)
            {
                response.Command = commands.ToDictionary(c => c.Key, c => c.Value.GetDisplayInfo()?.GetDescription());
                response.Description = Strings.MissingCommandName_Warning;
            }
            else if (commands.Count == 1)
            {
#if NETSTANDARD2_0
                var kv = commands.First();
                var name = kv.Key;
                var command = kv.Value;
#else
                var (name, command) = commands.First();
#endif
                response.Command = name;
                response.Description = command.GetDisplayInfo()?.GetDescription();
                response.Parameters = command.Parameters
                    .Select(this.GetParameterDescription)
                    .ToArray();
            }
            else
            {
                response.Command = null;
                response.Description = Strings.NoMatchingCommands_Warning.FormatWith(message.Command);
            }

            return Task.FromResult(response);
        }

        private string GetParameterDescription(IParameterInfo p)
        {
            var displayInfo = p.GetDisplayInfo();
            var shortName = displayInfo?.GetShortName();
            shortName = string.IsNullOrEmpty(shortName)
                ? string.Empty
                : $"/{shortName}";
            return $"{p.Name}{shortName} ({this.GetFormattedTypeName(p.ValueType)}): {displayInfo?.GetDescription()}";
        }

        private string GetFormattedTypeName(ITypeInfo typeInfo)
        {
            var type = typeInfo.AsType();
            var nonNullableType = type.GetNonNullableType();
            return type == nonNullableType ? $"{type}" : $"{nonNullableType}?";
        }
    }
}