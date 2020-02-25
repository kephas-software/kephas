// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProperty.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Model
{
    using Kephas.Reflection;

    /// <summary>
    /// Contract for properties.
    /// </summary>
    public interface IProperty : IModelElement, IPropertyInfo
    {
    }
}