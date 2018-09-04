// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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