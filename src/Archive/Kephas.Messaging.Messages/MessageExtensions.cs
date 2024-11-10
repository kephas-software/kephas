// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Reflection;

namespace Kephas.Messaging;

using System.Runtime.CompilerServices;

using Kephas.Messaging.Events;
using Kephas.Messaging.Messages;

/// <summary>
/// Extension methods for <see cref="IMessage"/>.
/// </summary>
public static class MessageExtensions
{
    private static readonly MethodInfo ToMessageMethod = ReflectionHelper.GetGenericMethodOf(_ => ToMessage<object>(null!));
    private static readonly MethodInfo ToEventMethod = ReflectionHelper.GetGenericMethodOf(_ => ToEvent<object>(null!));
        
    /// <summary>
    /// Converts the provided object to a message.
    /// </summary>
    /// <param name="data">The object to be converted.</param>
    /// <returns>
    /// The object as an <see cref="IMessage"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IMessageBase ToMessage<T>(this T data)
        where T : class
    {
        _ = data ?? throw new ArgumentNullException(nameof(data));

        var paramType = typeof(T);
        if (paramType == typeof(object))
        {
            var dataType = data.GetType();
            if (dataType != typeof(object))
            {
                var toMessage = ToMessageMethod.MakeGenericMethod(dataType);
                return toMessage.Call<IMessageBase>(null, data);
            }

            paramType = dataType;
        }
            
        var resultType = paramType.GetBaseConstructedGenericOf(typeof(IMessage<>));
        return resultType is null
            ? new MessageEnvelope<T>(data)
            : (IMessageBase)data;
    }

    /// <summary>
    /// Converts the provided object to an event.
    /// </summary>
    /// <param name="data">The object to be converted.</param>
    /// <returns>
    /// The object as an <see cref="IEvent"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEvent ToEvent<T>(this T data)
        where T : class
    {
        _ = data ?? throw new ArgumentNullException(nameof(data));

        if (data is IEvent dataEvent)
        {
            return dataEvent;
        }

        var paramType = typeof(T);
        if (paramType == typeof(object))
        {
            paramType = data.GetType();
            if (paramType != typeof(object))
            {
                var toEvent = ToEventMethod.MakeGenericMethod(paramType);
                return toEvent.Call<IEvent>(null, data);
            }
        }

        return new EventEnvelope<T>(data);
    }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    /// <remarks>
    /// In case of a message envelope, it returns the contained message, otherwise the message itself.
    /// </remarks>
    /// <param name="message">The message to act on.</param>
    /// <returns>
    /// The message content.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object GetContent<T>([DisallowNull] this T message)
    {
        _ = message ?? throw new ArgumentNullException(nameof(message));
        return (message is IMessageEnvelopeBase envelope ? envelope.GetContent() : message);
    }
}