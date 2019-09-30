// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultCommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// A default command registry.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultCommandRegistry : ICommandRegistry
    {
        private readonly IAppRuntime appRuntime;
        private readonly ITypeLoader typeLoader;
        private IList<ITypeInfo> commandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCommandRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        public DefaultCommandRegistry(IAppRuntime appRuntime, ITypeLoader typeLoader)
        {
            this.appRuntime = appRuntime;
            this.typeLoader = typeLoader;
        }

        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <returns>
        /// The command types.
        /// </returns>
        public IEnumerable<ITypeInfo> GetCommandTypes()
        {
            return this.commandTypes ?? (this.commandTypes = this.appRuntime.GetAppAssemblies()
                                                                            .SelectMany(a => this.typeLoader.GetLoadableExportedTypes(a).Where(this.IsMessageType))
                                                                            .Select(t => (ITypeInfo)t.AsRuntimeTypeInfo())
                                                                            .ToList());
        }

        /// <summary>
        /// Resolves the command type.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// An ITypeInfo.
        /// </returns>
        public ITypeInfo ResolveCommandType(string command)
        {
            var commandTypes = this.GetCommandTypes();
            var commandType = (commandTypes.SingleOrDefault(m => m.Name.Equals(command, StringComparison.InvariantCultureIgnoreCase))
                               ?? commandTypes.SingleOrDefault(m => m.Name.Equals(command + "Message", StringComparison.InvariantCultureIgnoreCase)))
                              ?? commandTypes.SingleOrDefault(m => m.Name.Equals(command + "Event", StringComparison.InvariantCultureIgnoreCase));

            if (commandType == null)
            {
                var commands = this.commandTypes.Where(m => m.Name.StartsWith(command, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (commands.Count == 0)
                {
                    throw new InvalidOperationException($"Command type for '{command}' not found.");
                }

                if (commands.Count > 1)
                {
                    throw new AmbiguousMatchException($"Multiple command types found for '{command}': '{string.Join("', '", commands.Select(m => m.Name))}'.");
                }

                commandType = commands[0];
            }

            return commandType;
        }

        private bool IsMessageType(Type type) => !type.IsAbstract
                                                    && typeof(IMessage).IsAssignableFrom(type)
                                                    && !type.Name.EndsWith("ResponseMessage")
                                                    && !typeof(IMessageEnvelope).IsAssignableFrom(type)
                                                    && type.GetCustomAttribute<ExcludeFromModelAttribute>() == null;
    }
}
