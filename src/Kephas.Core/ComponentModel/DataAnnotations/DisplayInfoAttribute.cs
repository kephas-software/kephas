// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayInfoAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the display info attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ComponentModel.DataAnnotations
{
    using System;

    using Kephas.Localization.Internal;
    using Kephas.Reflection;
    using Kephas.Reflection.Localization;

    /// <summary>
    /// Display attribute for types.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class DisplayInfoAttribute : Attribute, IDisplayInfo
    {
        private readonly LocalizableString description = new LocalizableString(nameof(Description));
        private readonly LocalizableString name = new LocalizableString(nameof(Name));
        private readonly LocalizableString shortName = new LocalizableString(nameof(ShortName));
        private readonly LocalizableString prompt = new LocalizableString(nameof(Prompt));

        /// <summary>
        /// The resource type.
        /// </summary>
        private Type? resourceType;

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        /// <value>
        /// The resource type.
        /// </value>
        public Type? ResourceType
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
                this.shortName.ResourceType = value;
                this.prompt.ResourceType = value;
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
        /// Gets or sets the prompt.
        /// </summary>
        /// <value>
        /// The prompt.
        /// </value>
        public string Prompt
        {
            get => this.prompt.Value;
            set
            {
                if (this.prompt.Value == value)
                {
                    return;
                }

                this.prompt.Value = value;
            }
        }

        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        /// <value>
        /// The short name.
        /// </value>
        public string ShortName
        {
            get => this.shortName.Value;
            set
            {
                if (this.shortName.Value == value)
                {
                    return;
                }

                this.shortName.Value = value;
            }
        }

        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        public string? GetName()
        {
            return this.name.GetLocalizableValue();
        }

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        public string? GetDescription()
        {
            return this.description.GetLocalizableValue();
        }

        /// <summary>
        /// Gets the localized prompt.
        /// </summary>
        /// <returns>
        /// The localized prompt.
        /// </returns>
        public string? GetPrompt()
        {
            return this.prompt.GetLocalizableValue();
        }

        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <returns>
        /// The localized short name.
        /// </returns>
        public string? GetShortName()
        {
            return this.shortName.GetLocalizableValue();
        }
    }
}