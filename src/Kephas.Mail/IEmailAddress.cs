// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailAddress.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEmailAddress interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Mail
{
    using Kephas.Dynamic;

    /// <summary>
    /// Interface for email addresses.
    /// </summary>
    public interface IEmailAddress : IIndexable
    {
        /// <summary>
        /// Gets the address.
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Gets the display name of the subject associated to the address.
        /// </summary>
        string DisplayName { get; }
    }
}