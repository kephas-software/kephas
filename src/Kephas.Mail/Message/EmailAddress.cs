// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailAddress.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email address class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using Kephas.Dynamic;

    /// <summary>
    /// An email address.
    /// </summary>
    public class EmailAddress : Expando, IEmailAddress
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the display name of the subject associated to the address.
        /// </summary>
        public string DisplayName { get; set; }
    }
}