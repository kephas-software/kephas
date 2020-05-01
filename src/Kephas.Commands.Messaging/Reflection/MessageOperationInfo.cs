// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageOperationInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Commands.Messaging.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using Kephas.Runtime.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Provides metadata about an operation based on a message.
    /// </summary>
    public class MessageOperationInfo : DynamicOperationInfo, IPrototype
    {
        private readonly ITypeInfo messageType;
        private readonly Lazy<IMessageProcessor> lazyMessageProcessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOperationInfo"/> class.
        /// </summary>
        /// <param name="messageType">The message type.</param>
        /// <param name="lazyMessageProcessor">The message processor.</param>
        internal MessageOperationInfo(ITypeInfo messageType, Lazy<IMessageProcessor> lazyMessageProcessor)
        {
            Requires.NotNull(messageType, nameof(messageType));
            Requires.NotNull(lazyMessageProcessor, nameof(lazyMessageProcessor));

            this.messageType = messageType;
            this.lazyMessageProcessor = lazyMessageProcessor;
            var t = messageType;
            this.Name = t.Name.EndsWith("Message")
                ? t.Name.Substring(0, t.Name.Length - "Message".Length)
                : t.Name.EndsWith("Event")
                    ? t.Name.Substring(0, t.Name.Length - "Event".Length)
                    : t.Name;

            // ReSharper disable once VirtualMemberCallInConstructor
            this.FullName = $"{messageType.Namespace}.{this.Name}";

            this.Parameters = messageType.Properties
                .Select((p, i) => new MessageParameterInfo(this, p, i))
                .ToArray();

            messageType.Annotations.ForEach(this.AddAnnotation);
            this.ReturnType = this.Annotations.OfType<ReturnTypeAttribute>().FirstOrDefault()?.Value?.AsRuntimeTypeInfo();
        }

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public override IDisplayInfo? GetDisplayInfo()
        {
            return this.messageType.GetDisplayInfo();
        }

        /// <summary>
        /// Invokes the specified method on the provided instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The invocation result.</returns>
        public override object? Invoke(object? instance, IEnumerable<object?> args)
        {
            var argsList = args?.ToArray() ?? Array.Empty<object?>();

            var values = argsList.Length > 0 ? (IExpando?)argsList[0] : null;
            var opContext = argsList.Length > 1 ? (IContext?)argsList[1] : null;
            var opToken = argsList.Length > 2 ? (CancellationToken)argsList[2] : default;

            var message = this.CreateMessage(values);
            var operation = new MessageOperation(message, this.lazyMessageProcessor);
            return operation.ExecuteAsync(opContext, opToken);
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">Optional. The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public object CreateInstance(IEnumerable<object?>? args = null)
        {
            var values = (IExpando?)args?.FirstOrDefault();
            var message = this.CreateMessage(values);

            return new MessageOperation(message, this.lazyMessageProcessor);
        }

        /// <summary>
        /// Creates the message based on the provided values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The message.</returns>
        protected internal virtual object CreateMessage(IExpando? values)
        {
            var message = this.messageType.CreateInstance();

            if (values == null)
            {
                return message;
            }

            var i = 0;
            var messageProperties = this.messageType.Properties.ToList();
            foreach (var kv in this.GetMessageArguments(values))
            {
                var propInfo = messageProperties.FirstOrDefault(
                    p => string.Equals(p.Name, kv.Key, StringComparison.OrdinalIgnoreCase)
                                                || string.Equals(p.GetDisplayInfo()?.GetShortName(), kv.Key, StringComparison.OrdinalIgnoreCase));
                if (propInfo == null)
                {
                    if (!this.HandleUnknownArgument(message, i, kv.Key, kv.Value))
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
        /// Gets message arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The message arguments.
        /// </returns>
        protected virtual IDictionary<string, object?> GetMessageArguments(IExpando args) => args.ToDictionary();

        /// <summary>
        /// Handles an argument that cannot be matched by name.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="index">Zero-based index of the argument.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        protected virtual bool HandleUnknownArgument(object message, int index, string name, object? value)
        {
            var props = this.messageType.Properties.ToList();
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
    }
}