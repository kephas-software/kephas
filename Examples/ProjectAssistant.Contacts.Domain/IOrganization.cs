// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines an organization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProjectAssistant.Contacts.Domain
{
    /// <summary>
    /// Defines an organization.
    /// </summary>
    public interface IOrganization : IContact
    {
        /// <summary>
        /// Gets or sets the VAT identifier.
        /// </summary>
        /// <value>
        /// The VAT identifier.
        /// </value>
        string VatId { get; set; }
    }
}