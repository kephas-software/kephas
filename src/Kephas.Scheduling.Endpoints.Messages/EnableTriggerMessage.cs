// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableTriggerMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enable trigger message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Kephas.Scheduling.Endpoints
{
    using System.ComponentModel.DataAnnotations;

    using Kephas.Messaging;

    /// <summary>
    /// An enable trigger message.
    /// </summary>
    [Display(Description = "Enables the specified trigger.")]
    public class EnableTriggerMessage : IActionMessage
    {
    }
}
