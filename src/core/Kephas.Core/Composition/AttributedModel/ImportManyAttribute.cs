// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportManyAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Specifies that a property, field, or parameter imports a particular set of exports.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.AttributedModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Specifies that a property, field, or parameter imports a particular set of exports.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Should be able to inherit to allow custom metadata.")]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter,
                    AllowMultiple = false, Inherited = false)]
    public class ImportManyAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the 
        ///     set of exports without a contract name.
        /// </summary>
        public ImportManyAttribute()
            : this(null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportManyAttribute"/> class, importing the
        ///     set of exports with the specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the exports to import, or 
        ///      <see langword="null"/>.
        /// </param>
        public ImportManyAttribute(string contractName)
        {
            this.ContractName = contractName;
        }

        /// <summary>
        ///     Gets the contract name of the exports to import.
        /// </summary>
        /// <value>
        ///      A <see cref="String"/> containing the contract name of the exports to import. The 
        ///      default value is null.
        /// </value>
        public string ContractName { get; private set; }
    }
}