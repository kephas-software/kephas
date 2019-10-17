// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HelpMessageHandler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the help message handler class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console.Endpoints
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Messaging;
    using Kephas.Reflection;

    /// <summary>
    /// A help message handler.
    /// </summary>
    public class HelpMessageHandler : MessageHandlerBase<HelpMessage, HelpResponseMessage>
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpMessageHandler"/> class.
        /// </summary>
        /// <param name="commandRegistry">The command registry.</param>
        public HelpMessageHandler(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
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
            var commandTypes = this.commandRegistry.GetCommandTypes().ToList();

            var commands = commandTypes.ToDictionary(
                t => t.Name.EndsWith("Message")
                         ? t.Name.Substring(0, t.Name.Length - "Message".Length)
                         : t.Name.EndsWith("Event")
                             ? t.Name.Substring(0, t.Name.Length - "Event".Length)
                             : t.Name,
                t => t).ToList();

            var response = new HelpResponseMessage();

            commands = string.IsNullOrEmpty(message.Command)
                           ? commands
                           : commands.Where(
                                   kv => kv.Key.StartsWith(
                                       message.Command,
                                       StringComparison.InvariantCultureIgnoreCase))
                               .ToList();

            if (commands.Count > 1)
            {
                response.Command = commands.OrderBy(t => t.Key).ToDictionary(c => c.Key, c => c.Value.GetLocalization().Description).ToArray();
                response.Description = "Please provide one command name to see the information about that command. Example: help command=help.";
            }
            else if (commands.Count == 1)
            {
                var cmd = commands[0];
                response.Command = commands.Select(t => t.Key).Single();
                var localization = cmd.Value.GetLocalization();
                response.Description = localization.Description;
                response.Parameters = cmd.Value.Properties.Select(p => $"{p.Name} ({p.ValueType}): {p.GetLocalization().Description}").ToArray();
            }
            else
            {
                response.Command = null;
                response.Description = $"No commands with the filter '{message.Command}*' found.";
            }

            return Task.FromResult(response);
        }
    }
}