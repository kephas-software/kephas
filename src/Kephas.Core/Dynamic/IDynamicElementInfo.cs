// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDynamicElementInfo.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDynamicElementInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Dynamic
{
    using System.Reflection;

    /// <summary>
    /// Interface for dynamic element information.
    /// </summary>
    public interface IDynamicElementInfo
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