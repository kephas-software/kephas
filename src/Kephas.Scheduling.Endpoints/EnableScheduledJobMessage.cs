// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnableScheduledJobMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Messaging;

    /// <summary>
    /// Message for enabling a scheduled job.
    /// </summary>
    [DisplayInfo(Description = "Enables the specified scheduled job.")]
    public class EnableScheduledJobMessage : IMessage
    {
    }
}