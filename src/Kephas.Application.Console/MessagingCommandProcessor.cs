// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagingCommandProcessor.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the messaging command processor class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Command processor based on messaging. It packs the command line in messages and delegates execution to the <see cref="IMessageProcessor"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class MessagingCommandProcessor : ICommandProcessor
    {
        private readonly ICommandRegistry registry;
        private readonly ICommandIdentityResolver identityResolver;
        private readonly IMessageProcessor messageProcessor;
        private readonly IContextFactory contextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingCommandProcessor"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <param name="identityResolver">The identity resolver.</param>
        /// <param name="messageProcessor">The message processor.</param>
        /// <param name="contextFactory">The context factory.</param>
        public MessagingCommandProcessor(ICommandRegistry registry, ICommandIdentityResolver identityResolver, IMessageProcessor messageProcessor, IContextFactory contextFactory)
        {
            Requires.NotNull(registry, nameof(registry));
            Requires.NotNull(identityResolver, nameof(identityResolver));
            Requires.NotNull(messageProcessor, nameof(messageProcessor));
            Requires.NotNull(contextFactory, nameof(contextFactory));

            this.registry = registry;
            this.identityResolver = identityResolver;
            this.messageProcessor = messageProcessor;
            this.contextFactory = contextFactory;
        }

        /// <summary>
        /// Executes the asynchronous operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<object> ProcessAsync(string command, IExpando args = null, CancellationToken cancellationToken = default)
        {
            var message = this.CreateMessage(command, args = args ?? new Expando());
            return this.ProcessMessageAsync(message, args, cancellationToken);
        }

        /// <summary>
        /// Creates the message from the command and the arguments.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new message.
        /// </returns>
        protected virtual IMessage CreateMessage(string command, IExpando args)
        {
            var commandType = this.registry.ResolveCommandType(command);

            var message = (IMessage)commandType.CreateInstance();
            var i = 0;
            foreach (var kv in this.GetMessageArguments(args))
            {
                var propInfo = commandType.Properties.FirstOrDefault(p => string.Compare(p.Name, kv.Key, StringComparison.OrdinalIgnoreCase) == 0);
                if (propInfo == null)
                {
                    if (!this.HandleUnknownArgument(message, commandType, i, kv.Key, kv.Value))
                    {
                        throw new InvalidOperationException($"Parameter '{kv.Key} (index: {i})' not found.");
                    }
                }
                else
                {
                    this.SetPropertyValue(message, propInfo, kv.Value);
                }

                i++;
            }

            return message;
        }

        /// <summary>
        /// Handles an argument that cannot be matched by name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="index">Zero-based index of the argument.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        protected virtual bool HandleUnknownArgument(IMessage message, ITypeInfo messageType, int index, string name, object value)
        {
            var props = messageType.Properties.ToList();
            if (index < props.Count && (value == null || (value is string stringValue && string.IsNullOrEmpty(stringValue))))
            {
                this.SetPropertyValue(message, props[index], name);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the message property value.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="propInfo">Information describing the property.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetPropertyValue(IMessage message, IPropertyInfo propInfo, object value)
        {
            var propValueType = (propInfo.ValueType as IRuntimeTypeInfo).Type;
            var convertedValue = Convert.ChangeType(value, propValueType);
            propInfo.SetValue(message, convertedValue);
        }

        /// <summary>
        /// Gets message arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The message arguments.
        /// </returns>
        protected virtual IDictionary<string, object> GetMessageArguments(IExpando args) => args.ToDictionary();

        /// <summary>
        /// Process the message asynchronously.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the process message.
        /// </returns>
        protected virtual async Task<object> ProcessMessageAsync(IMessage message, IExpando args, CancellationToken cancellationToken)
        {
            using (var context = this.contextFactory.CreateContext<MessagingContext>(this.messageProcessor, message))
            {
                context.Identity = this.identityResolver.ResolveIdentity(context);
                var result = await this.messageProcessor.ProcessAsync(message, context, cancellationToken).PreserveThreadContext();
                return result;
            }
        }
    }
}
