// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAddress.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Defines an address.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Foundation.Domain.Contacts
{
    using Kephas.Data.Model.Abstractions;

    /// <summary>
    /// Defines an address.
    /// </summary>
    public interface IAddress : IIdentifiable<long>, INamed
    {
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>
        /// The street.
        /// </value>
        string Street { get; set; }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        string No { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        /// <value>
        /// The locality.
        /// </value>
        string Locality { get; set; }

        /// <summary>
        /// Gets or sets the zip code.
        /// </summary>
        /// <value>
        /// The zip code.
        /// </value>
        string ZipCode { get; set; }

        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the country.
        /// </summary>
        /// <value>
        /// The name of the country.
        /// </value>
        string CountryName { get; set; }
    }
}