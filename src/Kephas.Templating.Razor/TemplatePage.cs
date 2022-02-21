// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatePage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating.Razor
{
    using Microsoft.AspNetCore.Mvc.Razor;

    /// <summary>
    /// The base template page class.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public abstract class TemplatePage<T> : RazorPage<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePage{T}"/> class.
        /// </summary>
        protected TemplatePage()
        {
            this.HtmlEncoder = System.Text.Encodings.Web.HtmlEncoder.Default;
        }
    }
}
