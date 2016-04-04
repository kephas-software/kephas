// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines an organization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleModel.Contacts.Domain
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