// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INonIdentifiableMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Messages
{
    /// <summary>
    /// Interface for messages that are not identifiable, in the sense
    /// that their identifier, if it exists, cannot be used as a logical behavior/handler discriminator.
    /// </summary>
    public interface INonIdentifiableMessage : IMessage
    {
    }
}