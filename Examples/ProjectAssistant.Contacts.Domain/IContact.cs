// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContact.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a contact person.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProjectAssistant.Contacts.Domain
{
    using Kephas.Data;
    using Kephas.Model.AttributedModel.Behaviors;

    /// <summary>
    /// Defines a contact person.
    /// </summary>
    public interface IContact : IIdentifiable
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