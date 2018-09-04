// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeDisplayAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type display attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ComponentModel.DataAnnotations
{
    using System;

    using Kephas.Localization.Internal;

    /// <summary>
    /// Display attribute for types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum, AllowMultiple = false)]
    public class TypeDisplayAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        private readonly LocalizableString description = new LocalizableString(nameof(Description));

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        private readonly LocalizableString name = new LocalizableString(nameof(Name));

        /// <summary>
        /// The resource type.
        /// </summary>
        private Type resourceType;

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        /// <value>
        /// The resource type.
        /// </value>
        public Type ResourceType
        {
            get => this.resourceType;
            set
            {
                if (this.resourceType == value)
                {
                    return;
                }

                this.resourceType = value;
                this.name.ResourceType = value;
                this.description.ResourceType = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get => this.name.Value;
            set
            {
                if (this.name.Value == value)
                {
                    return;
                }

                this.name.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get => this.description.Value;
            set
            {
                if (this.description.Value == value)
                {
                    return;
                }

                this.description.Value = value;
            }
        }

        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        public string GetName()
        {
            return this.name.GetLocalizableValue();
        }

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        public string GetDescription()
        {
            return this.description.GetLocalizableValue();
        }
    }
}