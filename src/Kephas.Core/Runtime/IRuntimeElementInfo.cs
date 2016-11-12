// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRuntimeElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IRuntimeElementInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime
{
    using System.Reflection;

    /// <summary>
    /// Interface for dynamic element information.
    /// </summary>
    public interface IRuntimeElementInfo
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