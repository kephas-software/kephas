// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailSenderService.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEmailSenderService interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Services
{
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Contract services sending emails.
    /// </summary>
    [ContractClass(typeof(EmailSenderServiceCodeContract))]
    public interface IEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Contract class for <see cref="IEmailSenderService"/>
    /// </summary>
    [ContractClassFor(typeof(IEmailSenderService))]
    internal abstract class EmailSenderServiceCodeContract : IEmailSenderService
    {
        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="emailMessage">The email message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        public Task SendAsync(IEmailMessage emailMessage, CancellationToken cancellationToken = new CancellationToken())
        {
            Contract.Requires(emailMessage != null);
            Contract.Ensures(Contract.Result<Task>() != null);

            return Contract.Result<Task>();
        }
    }
}