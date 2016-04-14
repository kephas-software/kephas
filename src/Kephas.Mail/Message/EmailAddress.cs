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
        /// Gets the address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets the display name of the subject associated to the address.
        /// </summary>
        public string DisplayName { get; set; }
    }
}