// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Marks a classifier as being abstract (cannot be instantiated).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.AttributedModel
{
    using System;

    /// <summary>
    /// Marks a classifier as being abstract (cannot be instantiated).
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class AbstractAttribute : Attribute
    {
    }
}