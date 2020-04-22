// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementInfoLocalization.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the element information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Localization;
    using Kephas.Runtime;

    /// <summary>
    /// Localization information for <see cref="IElementInfo"/>.
    /// </summary>
    public class ElementInfoLocalization : Localization, IElementInfoLocalization
    {
        private string name;
        private string description;
        private string shortName;
        private string prompt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInfoLocalization"/> class.
        /// </summary>
        public ElementInfoLocalization()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.DisplayInfo = this.GetDisplayInfo(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInfoLocalization"/> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        public ElementInfoLocalization(IElementInfo elementInfo)
        {
            Requires.NotNull(elementInfo, nameof(elementInfo));

            // ReSharper disable once VirtualMemberCallInConstructor
            this.DisplayInfo = this.GetDisplayInfo(elementInfo);
        }

        /// <summary>
        /// Gets or sets the localized name.
        /// </summary>
        /// <value>
        /// The localized name.
        /// </value>
        public virtual string Name
        {
            get => this.DisplayInfo == null
                ? this.name
                : this.DisplayInfo.GetName();
            set => this.name = value;
        }

        /// <summary>
        /// Gets or sets the localized description.
        /// </summary>
        /// <value>
        /// The localized description.
        /// </value>
        public virtual string Description
        {
            get => this.DisplayInfo == null
                ? this.description
                : this.DisplayInfo.GetDescription();
            set => this.description = value;
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
            get => this.DisplayInfo == null
                ? this.shortName
                : this.DisplayInfo.GetShortName();
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
            get => this.DisplayInfo == null
                ? this.prompt
                : this.DisplayInfo.GetPrompt();
            set => this.prompt = value;
        }

        /// <summary>
        /// Gets the <see cref="IDisplayInfo"/> used to extract the localized values.
        /// </summary>
        protected IDisplayInfo? DisplayInfo { get; }

        /// <summary>
        /// Tries to get the display attribute from the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <returns>
        /// A <see cref="IDisplayInfo"/> attribute or <c>null</c>.
        /// </returns>
        protected virtual IDisplayInfo? GetDisplayInfo(IElementInfo? elementInfo) =>
            elementInfo?.GetDisplayInfo();
    }
}