// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProject.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines a project.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ProjectAssistant.ProjectManagement.Domain
{
    using Kephas.Data;
    using Kephas.Model.AttributedModel.Behaviors;

    using ProjectAssistant.Contacts.Domain;

    /// <summary>
    /// Defines a project.
    /// </summary>
    public interface IProject : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the project beneficiary.
        /// </summary>
        /// <value>
        /// The beneficiary.
        /// </value>
        [Required]
        IRef<IOrganization> Beneficiary { get; set; }

        /// <summary>
        /// Gets or sets the general consultant.
        /// </summary>
        /// <value>
        /// The general consultant.
        /// </value>
        [Required]
        IRef<IOrganization> GeneralConsultant { get; set; }
    }
}