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
    }
}