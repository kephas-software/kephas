﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the type information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// The type info localization.
    /// </summary>
    public class TypeInfoLocalization : ElementInfoLocalization, ITypeInfoLocalization
    {
        /// <summary>
        /// The properties' localizations.
        /// </summary>
        private IDictionary<string, IPropertyInfoLocalization> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLocalization"/> class.
        /// </summary>
        public TypeInfoLocalization()
        {
            this.properties = new Dictionary<string, IPropertyInfoLocalization>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInfoLocalization"/> class.
        /// </summary>
        /// <param name="typeInfo">Information describing the type.</param>
        public TypeInfoLocalization(ITypeInfo typeInfo)
            : base(typeInfo)
        {
            this.properties = typeInfo.Properties.ToDictionary(p => p.Name, this.CreatePropertyInfoLocalization);
        }

        /// <summary>
        /// Gets or sets a dictionary of properties' localizations.
        /// </summary>
        /// <value>
        /// The properties' localizations.
        /// </value>
        public IDictionary<string, IPropertyInfoLocalization> Properties
        {
            get => this.properties;
            set
            {
                Requires.NotNull(value, nameof(value));

                this.properties = value;
            }
        }

        /// <summary>
        /// Creates a property information localization.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        /// <returns>
        /// The new property information localization.
        /// </returns>
        protected virtual IPropertyInfoLocalization CreatePropertyInfoLocalization(IPropertyInfo propertyInfo)
        {
            return new PropertyInfoLocalization(propertyInfo);
        }
    }
}