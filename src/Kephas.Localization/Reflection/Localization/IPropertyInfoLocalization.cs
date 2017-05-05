// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyInfoLocalization.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IPropertyInfoLocalization interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection.Localization
{
    /// <summary>
    /// Interface for property information localization.
    /// </summary>
    public interface IPropertyInfoLocalization : IElementInfoLocalization
    {
        /// <summary>
        /// Gets the localized short name.
        /// </summary>
        /// <remarks>
        /// The short name can be used for example in column headers.
        /// </remarks>
        /// <value>
        /// The localized short name.
        /// </value>
        string ShortName { get; }

        /// <summary>
        /// Gets the localized value that will be used to set the watermark for prompts in the UI.
        /// </summary>
        /// <value>
        /// The localized value that will be used to set the watermark for prompts in the UI.
        /// </value>
        string Prompt { get; }
    }
}