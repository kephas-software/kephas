// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRefPropertyInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Reflection
{
    using Kephas.Reflection;

    /// <summary>
    /// Property information indicating a reference to another entity.
    /// </summary>
    public interface IRefPropertyInfo : IPropertyInfo
    {
        /// <summary>
        /// Gets the reference type.
        /// </summary>
        ITypeInfo RefType { get; }
    }
}