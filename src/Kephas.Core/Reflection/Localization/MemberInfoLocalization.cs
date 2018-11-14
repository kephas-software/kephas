// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemberInfoLocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public class MemberInfoLocalization : ElementInfoLocalization, IMemberInfoLocalization
    {
        /// <summary>
        /// The localized short name.
        /// </summary>
        private string shortName;

        /// <summary>
        /// The localized prompt.
        /// </summary>
        private string prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberInfoLocalization"/> class.
        /// </summary>
        public MemberInfoLocalization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberInfoLocalization"/> class.
        /// </summary>
        /// <param name="memberInfo">Information describing the member.</param>
        public MemberInfoLocalization(IElementInfo memberInfo)
            : base(memberInfo)
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