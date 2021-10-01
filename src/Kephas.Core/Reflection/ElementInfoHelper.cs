// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementInfoHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Kephas.ComponentModel.DataAnnotations;
    using Kephas.Runtime;

    /// <summary>
    /// Extension methods for <see cref="IElementInfo"/>.
    /// </summary>
    public static class ElementInfoHelper
    {
        /// <summary>
        /// The display info key.
        /// </summary>
        public const string DisplayInfoKey = "__DisplayInfo";

        /// <summary>
        /// Tries to compute the <see cref="IDisplayInfo"/> from the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <returns>
        /// A <see cref="IDisplayInfo"/> or <c>null</c>.
        /// </returns>
        public static IDisplayInfo? GetDisplayInfo(IElementInfo? elementInfo)
        {
            if (elementInfo == null)
            {
                return null;
            }

            if (elementInfo[DisplayInfoKey] is IDisplayInfo displayInfo)
            {
                return displayInfo;
            }

            var annotations = elementInfo?.Annotations;
            displayInfo = annotations?.OfType<IDisplayInfo>().FirstOrDefault();
            if (displayInfo == null)
            {
                displayInfo = annotations
                    ?.OfType<IAttributeProvider>()
                    .Select(p => p.GetAttribute<DisplayInfoAttribute>())
                    .FirstOrDefault(a => a != null);

                if (displayInfo == null)
                {
                    var displayAttribute = ComputeDisplayAttribute(elementInfo);
                    if (displayAttribute != null)
                    {
                        displayInfo = new DisplayAttributeAdapter(displayAttribute);
                    }
                }
            }

            if (displayInfo == null)
            {
                displayInfo = new DisplayInfoAttribute();
            }

            elementInfo[DisplayInfoKey] = displayInfo;
            return displayInfo;
        }

        /// <summary>
        /// Tries to get the display attribute from the provided <see cref="IElementInfo"/>.
        /// </summary>
        /// <param name="elementInfo">Information describing the element.</param>
        /// <returns>
        /// A <see cref="IDisplayInfo"/> attribute or <c>null</c>.
        /// </returns>
        private static DisplayAttribute? ComputeDisplayAttribute(IElementInfo? elementInfo)
        {
            if (elementInfo == null)
            {
                return null;
            }

            var annotations = elementInfo?.Annotations;
            var displayAttribute = annotations?.OfType<DisplayAttribute>().FirstOrDefault();
            if (displayAttribute != null)
            {
                return displayAttribute;
            }

            displayAttribute = annotations
                ?.OfType<IAttributeProvider>()
                .Select(p => p.GetAttribute<DisplayAttribute>())
                .FirstOrDefault(a => a != null);

            return displayAttribute;
        }
    }
}