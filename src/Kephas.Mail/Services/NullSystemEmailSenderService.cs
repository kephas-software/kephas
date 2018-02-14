// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullSystemEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null system email sender service class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A null system email sender service.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullSystemEmailSenderService : ISystemEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="context">The sending context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        public Task SendAsync(
            IEmailMessage emailMessage,
            IContext context = null,
            CancellationToken cancellationToken = default)
        {
            return TaskHelper.CompletedTask;
        }

        /// <summary>
        /// Creates email message builder.
        /// </summary>
        /// <param name="context">The sending context.</param>
        /// <returns>
        /// The new email message builder.
        /// </returns>
        public IEmailMessageBuilder CreateEmailMessageBuilder(IContext context = null)
        {
            return new NullEmailMessageBuilder();
        }
    }
}