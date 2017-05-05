// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the element information localization class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Localization;

    /// <summary>
    /// Localization information for <see cref="IElementInfo"/>.
    /// </summary>
    public class ElementInfoLocalization : Localization, IElementInfoLocalization
    {
        /// <summary>
        /// The localized name.
        /// </summary>
        private string name;

        /// <summary>
        /// The localized description.
        /// </summary>
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInfoLocalization"/> class.
        /// </summary>
        public ElementInfoLocalization()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.DisplayAttribute = this.TryGetDisplayAttribute(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementInfoLocalization"/> class.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        public ElementInfoLocalization(IElementInfo elementInfo)
        {
            Requires.NotNull(elementInfo, nameof(elementInfo));

            // ReSharper disable once VirtualMemberCallInConstructor
            this.DisplayAttribute = this.TryGetDisplayAttribute(elementInfo);
        }

        /// <summary>
        /// Gets or sets the localized name.
        /// </summary>
        /// <value>
        /// The localized name.
        /// </value>
        public virtual string Name
        {
            get => this.DisplayAttribute == null ? this.name : this.DisplayAttribute.GetName();
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
            get => this.DisplayAttribute == null ? this.description : this.DisplayAttribute.GetDescription();
            set => this.description = value;
        }

        /// <summary>
        /// Gets the display attribute used to extract the localized values.
        /// </summary>
        protected DisplayAttribute DisplayAttribute { get; }

        /// <summary>
        /// Tries to get the display attribute from the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <returns>
        /// A DisplayAttribute or <c>null</c>.
        /// </returns>
        protected virtual DisplayAttribute TryGetDisplayAttribute(IElementInfo elementInfo)
        {
            return elementInfo?.Annotations.OfType<DisplayAttribute>().FirstOrDefault();
        }
    }
}