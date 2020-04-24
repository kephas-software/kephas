// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandProcessorBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the base class for command processors.
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
    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Base implementation of a command processor.
    /// </summary>
    public abstract class CommandProcessorBase : ICommandProcessor
    {
        private readonly ICommandRegistry registry;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProcessorBase"/> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        public CommandProcessorBase(ICommandRegistry registry)
        {
            Requires.NotNull(registry, nameof(registry));

            this.registry = registry;
        }

        /// <summary>
        /// Executes the asynchronous operation.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">Optional. The arguments.</param>
        /// <param name="context">Optional. The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// The asynchronous result.
        /// </returns>
        public Task<object?> ProcessAsync(string command, IExpando? args = null, IContext? context = null, CancellationToken cancellationToken = default)
        {
            var message = this.CreateCommand(command, args ??= new Expando());
            return this.ProcessCommandAsync(message, args, context, cancellationToken);
        }

        /// <summary>
        /// Creates the command object from the command and the arguments.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        protected virtual object CreateCommand(string command, IExpando args)
        {
            var commandType = this.registry.ResolveCommandType(command);

            var message = commandType.CreateInstance();
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
        protected virtual bool HandleUnknownArgument(object message, ITypeInfo messageType, int index, string name, object value)
        {
            var props = messageType.Properties.ToList();
            if (index < props.Count && (value == null || (value is bool boolValue && boolValue)))
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
        protected virtual void SetPropertyValue(object message, IPropertyInfo propInfo, object? value)
        {
            var propValueType = propInfo.ValueType.AsType();
            var propBaseType = propValueType.GetNonNullableType();
            var convertedValue = propBaseType.IsEnum
                ? Enum.Parse(propBaseType, (string?)value, ignoreCase: true)
                : Convert.ChangeType(value, propBaseType);
            propInfo.SetValue(message, convertedValue);
        }

        /// <summary>
        /// Gets message arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The message arguments.
        /// </returns>
        protected virtual IDictionary<string, object?> GetMessageArguments(IExpando args) => args.ToDictionary();

        /// <summary>
        /// Process the command asynchronously (core implementation).
        /// </summary>
        /// <param name="command">The message.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the processing result.
        /// </returns>
        protected abstract Task<object?> ProcessCommandAsync(object command, IExpando args, IContext? context, CancellationToken cancellationToken);
    }
}
