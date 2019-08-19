// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContact.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a contact person.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Foundation.Domain.Contacts
{
    using Kephas.Foundation.Domain.Abstractions;
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Defines a contact person.
    /// </summary>
    [Abstract]
    public interface IContact : INamedEntityBase
    {
        /// <summary>
        /// Gets or sets the main address.
        /// </summary>
        /// <value>
        /// The main address.
        /// </value>
        IAddress MainAddress { get; set; }

        /// <summary>
        /// Gets or sets the legal identifier.
        /// </summary>
        /// <remarks>
        /// This can be the social security number for persons and the registration number for organizations.
        /// </remarks>
        /// <value>
        /// The legal identifier.
        /// </value>
        string LegalId { get; set; }
    }
}