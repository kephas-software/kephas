// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextTypeAttribute.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Composition.Metadata;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Attribute indicating the supported.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DataContextTypeAttribute : Attribute, IMetadataValue<Type>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextTypeAttribute"/> class.
        /// </summary>
        /// <param name="dataContextType">Type of the data context.</param>
        public DataContextTypeAttribute(Type dataContextType)
        {
            Requires.NotNull(dataContextType, nameof(dataContextType));

            this.Value = dataContextType;
        }

        /// <summary>
        /// Gets the associated data context type.
        /// </summary>
        public Type Value { get; }

        /// <summary>
        /// Gets the associated data context type.
        /// </summary>
        object IMetadataValue.Value => this.Value;
    }
}