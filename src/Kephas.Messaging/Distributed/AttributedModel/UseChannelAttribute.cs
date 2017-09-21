// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseChannelAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the use channel attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute indicating the channel to use.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UseChannelAttribute : Attribute
    {
    }
}