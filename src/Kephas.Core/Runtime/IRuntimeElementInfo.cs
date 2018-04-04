// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IRuntimeElementInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// Interface for dynamic element information.
    /// </summary>
    public interface IRuntimeElementInfo : IElementInfo
    {
        /// <summary>
        /// Gets the underlying member information.
        /// </summary>
        /// <returns>
        /// The underlying member information.
        /// </returns>
        MemberInfo GetUnderlyingMemberInfo();
    }
}