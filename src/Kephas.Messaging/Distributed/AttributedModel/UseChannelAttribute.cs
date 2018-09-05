// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseChannelAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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