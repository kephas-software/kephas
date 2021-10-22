// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayAttributeAdapter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The display attribute adapter.
    /// </summary>
    public class DisplayAttributeAdapter : IDisplayInfo
    {
        private readonly DisplayAttribute displayAttr;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayAttributeAdapter"/> class.
        /// </summary>
        /// <param name="displayAttr">The display attribute.</param>
        public DisplayAttributeAdapter(DisplayAttribute displayAttr)
        {
            displayAttr = displayAttr ?? throw new ArgumentNullException(nameof(displayAttr));

            this.displayAttr = displayAttr;
        }

        /// <summary>
        /// Gets the display attribute.
        /// </summary>
        /// <value>
        /// The display attribute.
        /// </value>
        public DisplayAttribute DisplayAttribute => this.displayAttr;

        /// <summary>
        /// Gets the localized name.
        /// </summary>
        /// <returns>
        /// The localized name.
        /// </returns>
        public string? GetName() => this.displayAttr.GetName();

        /// <summary>
        /// Gets the localized description.
        /// </summary>
        /// <returns>
        /// The localized description.
        /// </returns>
        public string? GetDescription() => this.displayAttr.GetDescription();

        /// <summary>
        /// Gets the localized prompt.
        /// </summary>
        /// <returns>
        /// The localized prompt.
        /// </returns>
        public string? GetPrompt() => this.displayAttr.GetPrompt();

        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <returns>
        /// The localized short name.
        /// </returns>
        public string? GetShortName() => this.displayAttr.GetShortName();
    }
}