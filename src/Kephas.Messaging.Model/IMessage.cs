// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IMessage interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model
{
    using Kephas.Model;

    /// <summary>
    /// A message denotes classifiers holding metadata about the DTOs used in messaging.
    /// </summary>
    public interface IMessage : IClassifier
    {
    }
}