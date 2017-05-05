// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the property information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    /// <summary>
    /// Localization information for <see cref="IPropertyInfo"/>.
    /// </summary>
    public class PropertyInfoLocalization : ElementInfoLocalization, IPropertyInfoLocalization
    {
        /// <summary>
        /// The localized short name.
        /// </summary>
        private string shortName;

        private string prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfoLocalization"/> class.
        /// </summary>
        public PropertyInfoLocalization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfoLocalization"/> class.
        /// </summary>
        /// <param name="propertyInfo">Information describing the property.</param>
        public PropertyInfoLocalization(IPropertyInfo propertyInfo)
            : base(propertyInfo)
        {
        }

        /// <summary>
        /// Gets or sets the localized short name.
        /// </summary>
        /// <remarks>
        /// The short name can be used for example in column headers.
        /// </remarks>
        /// <value>
        /// The localized short name.
        /// </value>
        public string ShortName
        {
            get => this.DisplayAttribute == null ? this.shortName : this.DisplayAttribute.GetShortName();
            set => this.shortName = value;
        }

        /// <summary>
        /// Gets or sets the localized value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <value>
        /// The localized value that will be used to set the watermark for prompts in the UI.
        /// </value>
        public string Prompt
        {
            get => this.DisplayAttribute == null ? this.prompt : this.DisplayAttribute.GetPrompt();
            set => this.prompt = value;
        }
    }
}