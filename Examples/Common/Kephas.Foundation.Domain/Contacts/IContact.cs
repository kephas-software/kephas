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
    using System.ComponentModel.DataAnnotations;

    using Kephas.Data;
    using Kephas.Data.Model.Abstractions;

    /// <summary>
    /// Defines a contact person.
    /// </summary>
    [Abstract]
    public interface IContact : IIdentifiable<long>
    {
        /// <summary>
        /// Gets or sets the full name of the contact person.
        /// </summary>
        /// <value>
        /// The full name of the contact person.
        /// </value>
        [Required]
        string FullName { get; set; }

        /// <summary>
        /// Gets or sets the main address.
        /// </summary>
        /// <value>
        /// The main address.
        /// </value>
        IAddress MainAddress { get; set; } 
    }
}