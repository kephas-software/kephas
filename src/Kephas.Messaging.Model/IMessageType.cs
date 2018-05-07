// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessageType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model
{
    using Kephas.Model;

    /// <summary>
    /// A message type holds metadata about the DTOs used in messaging.
    /// </summary>
    public interface IMessageType : IClassifier
    {
    }
}