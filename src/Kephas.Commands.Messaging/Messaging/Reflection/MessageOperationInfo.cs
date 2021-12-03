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
    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Operations;
    using Kephas.Reflection;
    using Kephas.Reflection.Dynamic;
    using Kephas.Runtime;
    using Kephas.Runtime.AttributedModel;
    using Kephas.Services;

    /// <summary>
    /// Provides metadata about an operation based on a message.
    /// </summary>
    public class MessageOperationInfo : DynamicOperationInfo, IPrototype
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageOperationInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="lazyMessageProcessor">The message processor.</param>
        protected internal MessageOperationInfo(IRuntimeTypeRegistry typeRegistry, ITypeInfo messageType, Lazy<IMessageProcessor> lazyMessageProcessor)
        {
            messageType = messageType ?? throw new System.ArgumentNullException(nameof(messageType));
            lazyMessageProcessor = lazyMessageProcessor ?? throw new System.ArgumentNullException(nameof(lazyMessageProcessor));
            typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));

            this.MessageType = messageType;
            this.LazyMessageProcessor = lazyMessageProcessor;
            var t = messageType;
            this.Name = t.Name.EndsWith("Message")
                ? t.Name.Substring(0, t.Name.Length - "Message".Length)
                : t.Name.EndsWith("Event")
                    ? t.Name.Substring(0, t.Name.Length - "Event".Length)
                    : t.Name;

            // ReSharper disable once VirtualMemberCallInConstructor
            this.FullName = $"{messageType.Namespace}.{this.Name}";

            this.Parameters.AddRange(messageType.Properties
                .Select(this.CreateParameterInfo));

            this.Annotations.AddRange(messageType.Annotations);
            var returnType = this.Annotations.OfType<ReturnTypeAttribute>().FirstOrDefault()?.Value ?? typeof(object);
            this.ReturnType = typeRegistry.GetTypeInfo(returnType);
        }

        /// <summary>
        /// Gets the lazy <see cref="IMessageProcessor"/>.
        /// </summary>
        protected Lazy<IMessageProcessor> LazyMessageProcessor { get; }

        /// <summary>
        /// Gets the message type.
        /// </summary>
        protected ITypeInfo MessageType { get; }

        /// <summary>
        /// Gets the display information.
        /// </summary>
        /// <returns>The display information.</returns>
        public override IDisplayInfo? GetDisplayInfo()
        {
            return this.MessageType.GetDisplayInfo();
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

            var opArgs = argsList.Length > 0 ? (IDynamic?)argsList[0] : null;
            var opContext = argsList.Length > 1 ? (IContext?)argsList[1] : null;
            var opToken = argsList.Length > 2 ? (CancellationToken?)argsList[2] : default;

            var operation = (IOperation)this.CreateInstance(args);
            return operation.ExecuteAsync(opContext, opToken ?? default);
        }

        /// <summary>
        /// Creates an instance with the provided arguments (if any).
        /// </summary>
        /// <param name="args">Optional. The arguments.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        public virtual object CreateInstance(IEnumerable<object?>? args = null)
        {
            var opArgs = (IDynamic?)args?.FirstOrDefault();
            var message = this.CreateMessage(opArgs);

            return this.CreateOperation(message, opArgs);
        }

        /// <summary>
        /// Creates the operation based on the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The raw arguments.</param>
        /// <returns>The operation.</returns>
        protected internal virtual IOperation CreateOperation(object message, IDynamic? args)
        {
            return new MessageOperation(message, this.LazyMessageProcessor);
        }

        /// <summary>
        /// Creates the message based on the provided values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>The message.</returns>
        protected internal virtual object CreateMessage(IDynamic? values)
        {
            var message = this.MessageType.CreateInstance();

            if (values == null)
            {
                return message;
            }

            var i = 0;
            var messageProperties = this.MessageType.Properties.ToList();
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
        /// Creates a new parameter information out of the property information and position.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="position">The position.</param>
        /// <returns>The new <see cref="IParameterInfo"/>.</returns>
        protected virtual IParameterInfo CreateParameterInfo(IPropertyInfo propertyInfo, int position)
        {
            return new MessageParameterInfo(propertyInfo);
        }

        /// <summary>
        /// Gets message arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The message arguments.
        /// </returns>
        protected virtual IDictionary<string, object?> GetMessageArguments(IDynamic args) => args.ToDictionary();

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
            var props = this.MessageType.Properties.ToList();
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