// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProperty.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model
{
    using Kephas.Reflection;

    /// <summary>
    /// Contract for properties.
    /// </summary>
    public interface IProperty : IModelElement, IPropertyInfo
    {
    }

    public interface IParameter : IModelElement
    {

    }
}