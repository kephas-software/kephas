// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandRegistry.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default command registry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application;
    using Kephas.Commands.Messaging.Reflection;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Model.AttributedModel;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;

    /// <summary>
    /// A command registry for messages.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingCommandRegistry : ICommandRegistry
    {
        private IList<IOperationInfo>? commandTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandRegistry"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="typeLoader">The type loader.</param>
        /// <param name="lazyMessageProcessor">The lazy message processor.</param>
        /// <param name="typeRegistry">The type registry.</param>
        public MessagingCommandRegistry(
            IAppRuntime appRuntime,
            ITypeLoader typeLoader,
            Lazy<IMessageProcessor> lazyMessageProcessor,
            IRuntimeTypeRegistry typeRegistry)
        {
            this.AppRuntime = appRuntime;
            this.TypeLoader = typeLoader;
            this.TypeRegistry = typeRegistry;
            this.LazyMessageProcessor = lazyMessageProcessor;
        }

        /// <summary>
        /// Gets the application runtime.
        /// </summary>
        protected IAppRuntime AppRuntime { get; }

        /// <summary>
        /// Gets the type loader.
        /// </summary>
        protected ITypeLoader TypeLoader { get; }

        /// <summary>
        /// Gets the type registry.
        /// </summary>
        protected IRuntimeTypeRegistry TypeRegistry { get; }

        /// <summary>
        /// Gets the lazy message processor.
        /// </summary>
        protected Lazy<IMessageProcessor> LazyMessageProcessor { get; }

        /// <summary>
        /// Gets the command types.
        /// </summary>
        /// <param name="commandPattern">Optional. A pattern specifying the command types to retrieve.</param>
        /// <returns>
        /// The command types.
        /// </returns>
        public virtual IEnumerable<IOperationInfo> GetCommandTypes(string? commandPattern = null)
        {
            this.commandTypes ??= this.AppRuntime.GetAppAssemblies()
                                            .SelectMany(a => this.TypeLoader.GetExportedTypes(a).Where(this.IsMessageType))
                                            .Select(this.ToOperationInfo)
                                            .ToList()
                                            .AsReadOnly();

            return string.IsNullOrEmpty(commandPattern)
                ? this.commandTypes
                : this.commandTypes.Where(c => c.Name.StartsWith(commandPattern, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Converts the provided type to an <see cref="IOperationInfo"/>.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The <see cref="IOperationInfo"/>.</returns>
        protected virtual IOperationInfo ToOperationInfo(Type t) =>
            new MessageOperationInfo(this.TypeRegistry, this.TypeRegistry.GetTypeInfo(t), this.LazyMessageProcessor);

        private bool IsMessageType(Type type) => !type.IsAbstract
                                                 && typeof(IMessage).IsAssignableFrom(type)
                                                 && !typeof(IResponse).IsAssignableFrom(type)
                                                 && !type.Name.EndsWith("ResponseMessage")
                                                 && !typeof(IMessageEnvelope).IsAssignableFrom(type)
                                                 && type.GetCustomAttribute<ExcludeFromModelAttribute>() == null;
    }
}
