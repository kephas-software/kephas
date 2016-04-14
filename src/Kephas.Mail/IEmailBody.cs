// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailBody.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEmailBody interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail
{
    using Kephas.Dynamic;

    /// <summary>
    /// Enumerates email body types.
    /// </summary>
    public enum EmailBodyType
    {
        /// <summary>
        /// HTML body type.
        /// </summary>
        Html,

        /// <summary>
        /// Content body type.
        /// </summary>
        Text
    }

    /// <summary>
    /// The body of an Email address.
    /// </summary>
    public interface IEmailBody : IExpando
    {
        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Gets or sets the type of the body.
        /// </summary>
        EmailBodyType BodyType { get; set; }
    }
}