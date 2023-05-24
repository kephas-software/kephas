// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandlerAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message handler attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Messaging.AttributedModel;

/// <summary>
/// Adds message matching criteria for <see cref="IMessageHandler"/> services.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class MessageHandlerAttribute : Attribute, IMetadataProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageHandlerAttribute"/> class.
    /// </summary>
    /// <param name="messageTypeMatching">The message type matching.</param>
    public MessageHandlerAttribute(MessageTypeMatching messageTypeMatching)
    {
        this.MessageTypeMatching = messageTypeMatching;
    }

    /// <summary>
    /// Gets or sets the message type matching.
    /// </summary>
    /// <value>
    /// The message type matching.
    /// </value>
    public MessageTypeMatching MessageTypeMatching { get; set; }

    /// <summary>
    /// Gets or sets the envelope type.
    /// </summary>
    /// <value>
    /// The envelope type.
    /// </value>
    public Type? EnvelopeType { get; set; }

    /// <summary>
    /// Gets or sets the envelope type matching.
    /// </summary>
    /// <value>
    /// The envelope type matching.
    /// </value>
    public MessageTypeMatching EnvelopeTypeMatching { get; set; }

    /// <summary>
    /// Gets the metadata as an enumeration of (name, value) pairs.
    /// </summary>
    /// <returns>An enumeration of (name, value) pairs.</returns>
    public IEnumerable<(string name, object? value)> GetMetadata()
    {
        yield return (nameof(MessageHandlerMetadata.MessageTypeMatching), this.MessageTypeMatching);
        yield return (nameof(MessageHandlerMetadata.EnvelopeType), this.EnvelopeType);
        yield return (nameof(MessageHandlerMetadata.EnvelopeTypeMatching), this.EnvelopeTypeMatching);
    }
}