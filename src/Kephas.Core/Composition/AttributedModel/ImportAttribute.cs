﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Specifies that a property, field, or parameter imports a particular export.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.AttributedModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Specifies that a property, field, or parameter imports a particular export.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Should be able to inherit to allow custom metadata.")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ImportAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the 
        ///     export without a contract name.
        /// </summary>
        public ImportAttribute()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class, importing the
        ///     export with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="string"/> containing the contract name of the export to import, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        public ImportAttribute(string contractName)
        {
            this.ContractName = contractName;
        }

        /// <summary>
        ///     Gets the contract name of the export to import.
        /// </summary>
        /// <value>
        ///      A <see cref="string"/> containing the contract name of the export to import. The 
        ///      default value is null.
        /// </value>
        public string ContractName { get; private set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the property, field or parameter will be left 
        ///     at its default value when an export with the contract name is not present in 
        ///     the container.
        /// </summary>
        public bool AllowDefault { get; set; }
    }
}