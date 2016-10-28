// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataRepositoryTypeAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data repository type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Composition
{
    using System;
    using System.Diagnostics.Contracts;
    using Kephas.Composition.Metadata;

    /// <summary>
    /// Attribute for data repository type.
    /// </summary>
    public class DataRepositoryTypeAttribute : IMetadataValue<Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRepositoryTypeAttribute"/> class.
        /// </summary>
        /// <param name="dataRepositoryType">Type of the data repository.</param>
        public DataRepositoryTypeAttribute(Type dataRepositoryType)
        {
            Contract.Requires(dataRepositoryType != null);

            this.Value = dataRepositoryType;
        }

        /// <summary>
        /// Gets the associated data repository type.
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Gets the associated data repository type.
        /// </summary>
        object IMetadataValue.Value => this.Value;
    }
}