// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdGeneratorSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the identifier generator settings class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;

    using Kephas.Configuration;

    /// <summary>
    /// Settings for the <see cref="IIdGenerator"/>.
    /// </summary>
    public class IdGeneratorSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the start epoch for the timestamp part of an ID - 2015-06-01.
        /// </summary>
        public DateTimeOffset StartEpoch { get; set; } = new DateTimeOffset(new DateTime(2019, 9, 1), TimeSpan.Zero);

        /// <summary>
        /// Gets or sets the length of the namespace identifier bits.
        /// </summary>
        /// <value>
        /// The length of the namespace identifier bits.
        /// </value>
        public int NamespaceIdentifierBitLength { get; set; } = 3;

        /// <summary>
        /// Gets or sets the length of the discriminator part bits.
        /// </summary>
        /// <value>
        /// The length of the discriminator part bits.
        /// </value>
        public int DiscriminatorBitLength { get; set; } = 7;

        /// <summary>
        /// Gets or sets the namespace integer value.
        /// </summary>
        /// <value>
        /// The namespace integer value.
        /// </value>
        public int Namespace { get; set; } = 0;
    }
}