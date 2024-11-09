// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutAttribute.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the out attribute class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.AttributedModel
{
    using System;

    /// <summary>
    /// Attribute for output parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
    public class OutAttribute : Attribute
    {
    }
}