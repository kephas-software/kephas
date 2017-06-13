// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessagePartAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the message part attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a navigation property as modelling a message part,
    /// meaning that the property content is an aggregated part of the declaring message.
    /// If applied to a message class, marks it as being an aggregate of another message.
    /// </summary>
    /// <remarks>
    /// Message parts can be both self contained messages or collections.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MessagePartAttribute : Attribute
    {
    }
}