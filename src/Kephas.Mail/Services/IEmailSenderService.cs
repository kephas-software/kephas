// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEmailSenderService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;

    /// <summary>
    /// Contract services sending emails.
    /// </summary>
    public interface IEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="context">The sending context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        Task SendAsync(
            IEmailMessage emailMessage,
            IContext context = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates email message builder.
        /// </summary>
        /// <param name="context">The sending context.</param>
        /// <returns>
        /// The new email message builder.
        /// </returns>
        IEmailMessageBuilder CreateEmailMessageBuilder(IContext context = null);
    }
}