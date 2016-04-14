// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmailBody.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the email body class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail.Message
{
    using Kephas.Dynamic;

    /// <summary>
    /// An email body.
    /// </summary>
    public class EmailBody : Expando, IEmailBody
    {
        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the type of the body.
        /// </summary>
        public EmailBodyType BodyType { get; set; }
    }
}