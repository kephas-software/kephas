// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagePartAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the message part attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.AttributedModel
{
    using System;

    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Marks a navigation property as modeling a message part,
    /// meaning that the property content is an aggregated part of the declaring message.
    /// If applied to a message class, marks it as being an aggregate of another message.
    /// </summary>
    /// <remarks>
    /// Message parts can be both self contained messages or collections.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MessagePartAttribute : PartAttribute
    {
    }
}