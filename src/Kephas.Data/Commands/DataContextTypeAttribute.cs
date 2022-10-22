// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextTypeAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context type attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Attribute indicating the supported data context.
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
            dataContextType = dataContextType ?? throw new ArgumentNullException(nameof(dataContextType));

            this.Value = dataContextType;
        }

        /// <summary>
        /// Gets the associated data context type.
        /// </summary>
        public Type Value { get; }
    }
}